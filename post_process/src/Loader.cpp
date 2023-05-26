#include "Data.h"
#include "AreaOfInterest.h"

#include <fstream>
#include <sstream>

bool loadAOIs(const std::string &filename, std::vector<AreaOfInterest> &aois);

bool readData(const std::string &filename, std::vector<DataItem> &items) {
    std::fstream file(filename, std::ios::in);
    if (!file.is_open()) {
        throw std::runtime_error("Can't open file " + filename);
        return false;
    }

    DataItem item;
    Fixation fixation;
    Saccade saccade;
    MeasurementType measurementType; // allow us to know if we push to a saccade or a fixation

    std::string lineInput;
    int lineNumber = 0;
    while (std::getline(file, lineInput)) {
        lineNumber++;
        std::stringstream ss(lineInput);
        std::string firstWord;
        ss >> firstWord;

        if (firstWord == "#item") {
            // if item is not empty, push it to the vector
            if (item.fileNumber != "") {
                items.push_back(item);
                item = DataItem();
            }
            std::string type, fileNum;
            ss >> type >> fileNum;
            item.type = type == "TextData" ? DataType::TextData : 
                type == "ImageData" ? DataType::ImageData : 
                    throw std::runtime_error("Item type unvalid at line " + std::to_string(lineNumber));
            item.fileNumber = fileNum;
            loadAOIs("input/aois/" + fileNum + ".aoi", item.aois);
        }
        else if (firstWord == "##fixations") {
            measurementType = MeasurementType::Fixation;
        }
        else if (firstWord == "##saccades") {
            measurementType = MeasurementType::Saccade;
        }
        else if (firstWord == "begin") {
            std::string xStr, yStr, tsStr;
            ss >> xStr >> yStr >> tsStr;
            float x, y;
            double ts;
            try {
                x = xStr.substr(2) == "NaN" ? 0 : std::stoi(xStr.substr(2));
                y = yStr.substr(2) == "NaN" ? 0 : std::stoi(yStr.substr(2));
                ts = tsStr.substr(3) == "NaN" ? 0 : std::stoi(tsStr.substr(3));
            }
            catch (std::exception &e) {
                throw std::runtime_error("Error parsing etd file, line " + std::to_string(lineNumber) + ": " + e.what());
                return false;
            }
            if (measurementType == MeasurementType::Fixation) {
                fixation.begin = {x, y, ts};
            }
            else if (measurementType == MeasurementType::Saccade) {
                saccade.begin = {x, y, ts};
            }
        }
        else if (firstWord == "data") {
            std::string xStr, yStr, tsStr;
            ss >> xStr >> yStr >> tsStr;
            float x, y;
            double ts;
            try {
                x = xStr.substr(2) == "NaN" ? 0 : std::stoi(xStr.substr(2));
                y = yStr.substr(2) == "NaN" ? 0 : std::stoi(yStr.substr(2));
                ts = tsStr.substr(3) == "NaN" ? 0 : std::stoi(tsStr.substr(3));
            }
            catch (std::exception &e) {
                throw std::runtime_error("Error parsing line " + std::to_string(lineNumber) + ": " + e.what());
                return false;
            }
            fixation.data.push_back({x, y, ts});
        }
        else if (firstWord == "end") {
            std::string xStr, yStr, tsStr;
            ss >> xStr >> yStr >> tsStr;
            float x, y;
            double ts;
            try {
                x = xStr.substr(2) == "NaN" ? 0 : std::stoi(xStr.substr(2));
                y = yStr.substr(2) == "NaN" ? 0 : std::stoi(yStr.substr(2));
                ts = tsStr.substr(3) == "NaN" ? 0 : std::stoi(tsStr.substr(3));
            }
            catch (std::exception &e) {
                throw std::runtime_error("Error parsing line " + std::to_string(lineNumber) + ": " + e.what());
                return false;
            }
            if (measurementType == MeasurementType::Fixation) {
                fixation.end = {x, y, ts};
                if (
                    ((fixation.end.timestamp - fixation.begin.timestamp) >= 50) && // only save fixation if it takes more than 50ms
                    (fixation.begin.timestamp > 0) // fixations that start at 0 are unvalid
                ) {
                    item.fixations.push_back(fixation);
                }
                fixation = Fixation();
            }
            else if (measurementType == MeasurementType::Saccade) {
                saccade.end = {x, y, ts};
                item.saccades.push_back(saccade);
                saccade = Saccade();
            }
        }
    }
    items.push_back(item);
    return true;
}

bool loadAOIs(const std::string &filename, std::vector<AreaOfInterest> &aois) {
    std::fstream file(filename, std::ios::in);
    if (!file.is_open()) {
        // throw std::runtime_error("Can't open file " + filename);
        return false;
    }

    AOIType type;
    AreaOfInterest aoi;
    std::string lineInput;
    int lineNumber = 0;
    while (std::getline(file, lineInput)) {
        lineNumber++;
        std::stringstream ss(lineInput);
        std::string firstWord;
        ss >> firstWord;

        if (firstWord.substr(5) == "Rectangle") {
            type = AOIType::Rectangle;

            // get data as strings
            std::string name;
            std::string xstr, ystr;
            std::string widthStr, heightStr;
            ss >> name >> xstr >> ystr >> widthStr >> heightStr;
            // convert them to ints
            int x, y, width, height;
            try {
                x = stoi(xstr.substr(2));
                y = stoi(ystr.substr(2));
                width = stoi(widthStr.substr(2));
                height = stoi(heightStr.substr(2));
            }
            catch(std::exception &e) {
                throw std::runtime_error("Error parsing aoi file at line " + std::to_string(lineNumber) + ": " + e.what());
                return false;
            }
            name = name.substr(5);
            // Surface rect = {.x = x, .y = y, .width = width, .height = height};
            Surface rect = {.x = x, .y = y};
            rect.width = width;
            rect.height = height;
            AreaOfInterest aoi(type, rect, name);
            aois.push_back(aoi);
        }
        else if (firstWord.substr(5) == "Circle") {
            type = AOIType::Circle;

            // get data as strings
            std::string name;
            std::string xstr, ystr;
            std::string radiusStr;
            ss >> name >> xstr >> ystr >> radiusStr;
            // convert them to ints
            int x, y, radius;
            try {
                x = stoi(xstr.substr(2));
                y = stoi(ystr.substr(2));
                radius = stoi(radiusStr.substr(2));
            }
            catch(std::exception &e) {
                throw std::runtime_error("Error parsing aoi file at line " + std::to_string(lineNumber) + ": " + e.what());
                return false;
            }
            name = name.substr(5);
            Surface circle = {.x = x, .y = y};
            circle.radius = radius;
            AreaOfInterest aoi(type, circle, name);
            aois.push_back(aoi);
        }
    }

    return true;
}

