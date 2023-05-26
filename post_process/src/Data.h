#pragma once

#include <vector>
#include <string>

#include "AreaOfInterest.h"

struct DataPoint {
    float x;
    float y;
    double timestamp;
};


struct Fixation {
    DataPoint begin;
    std::vector<DataPoint> data;
    DataPoint end;
};

struct Saccade {
    DataPoint begin;
    DataPoint end;
};

enum class DataType { TextData, ImageData };
enum class MeasurementType { Fixation, Saccade };

struct DataItem {
    DataType type;
    std::string fileNumber;
    std::vector<AreaOfInterest> aois;

    std::vector<Fixation> fixations;
    std::vector<Saccade> saccades;
};

