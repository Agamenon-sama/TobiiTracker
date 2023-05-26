#include "AreaOfInterest.h"
#include "Data.h"

#include <fstream>
#include <sstream>

/* Rectangle::Rectangle(int x, int y, int width, int height) {
    _x = x;
    _y = y;
    _width = width;
    _height = height;
}

Circle::Circle(int x, int y, int radius) {
    _x = x;
    _y = y;
    _radius = radius;
} */

AreaOfInterest::AreaOfInterest(AOIType type, Surface surface, std::string name)
    :_type(type), _surface(surface), _name(name) {}

AreaOfInterest::~AreaOfInterest() {}

static DataPoint fixationAverage(const Fixation &fixation) {
    float x = fixation.begin.x;
    float y = fixation.begin.y;
    x += fixation.end.x;
    y += fixation.end.y;
    for (const auto &point : fixation.data) {
        x += point.x;
        y += point.y;
    }
    x /= fixation.data.size() + 2;
    y /= fixation.data.size() + 2;
    DataPoint result = {.x = x, .y = y};
    return result;
}

double AreaOfInterest::timeOfDwell(const std::vector<Fixation> &fixations) const {
    // define local lambda function to calculate the average (x,y) coordinates in a fixation
    // auto fixationAverage = [](const Fixation &fixation) {
    //     float x = fixation.begin.x;
    //     float y = fixation.begin.y;
    //     x += fixation.end.x;
    //     y += fixation.end.y;
    //     for (const auto &point : fixation.data) {
    //         x += point.x;
    //         y += point.y;
    //     }
    //     x /= fixation.data.size() + 2;
    //     y /= fixation.data.size() + 2;
    //     DataPoint result = {.x = x, .y = y};
    //     return result;
    // };

    // iterate over all fixations and if average of fixation is in AOI
    // we accumulate its duration
    double duration = 0.f;
    for (const auto &fixation : fixations) {
        auto average = fixationAverage(fixation);
        if (checkCollision(average.x, average.y)) {
            duration += fixation.end.timestamp - fixation.begin.timestamp;
        }
    }

    return duration;
}

double AreaOfInterest::timeToFirstFixation(const std::vector<Fixation> &fixations) const {
    double beginTime = fixations[0].begin.timestamp;

    for (const auto &fixation : fixations) {
        auto average = fixationAverage(fixation);
        if (checkCollision(average.x, average.y)) {
            // at the first collision, we return the time from the start
            // of the item until the beginning of that fixation
            return fixation.begin.timestamp - beginTime;
        }
    }

    return 0;
}

double AreaOfInterest::firstFixationDuration(const std::vector<Fixation> &fixations) const {
    for (const auto &fixation : fixations) {
        auto average = fixationAverage(fixation);
        if (checkCollision(average.x, average.y)) {
            return fixation.end.timestamp - fixation.begin.timestamp;
        }
    }

    return 0;
}

double AreaOfInterest::averageFixationDuration(const std::vector<Fixation> &fixations) const {
    std::vector<double> durations;
    
    // see which fixations collide with aoi and push its duration to vector
    for (const auto &fixation : fixations) {
       auto average = fixationAverage(fixation);
        if (checkCollision(average.x, average.y)) {
            // double ts = fixation.begin.timestamp;
            // ts += fixation.end.timestamp;
            // for (const auto &data : fixation.data) {
            //     ts += data.timestamp;
            // }
            // ts /= fixation.data.size() + 2;
            double duration = fixation.end.timestamp - fixation.begin.timestamp;
            durations.push_back(duration);
        } 
    }

    // calculate average of durations
    double average = 0;
    for (auto duration : durations) {
        average += duration;
    }
    if (durations.size() != 0) {
        average /= durations.size();
    }

    return average;
}

