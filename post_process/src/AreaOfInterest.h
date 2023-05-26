#pragma once

// #include "Data.h"

#include <string>
#include <stdexcept>
#include <vector>


/* class Surface {
    public:
    Surface() = default;
    virtual ~Surface() = default;
    virtual bool checkCollision(int x, int y) const {
        throw std::runtime_error("checkCollision for Surface is not implemented");
    }

    protected:
    int _x;
    int _y;
};
class Rectangle : public Surface {
    public:
    Rectangle() = default;
    Rectangle(int x, int y, int width, int height);
    bool checkCollision(int x, int y) const override {
        return (x > _x) && (x < _x + _width) && (y > _y) && (y < _y + _height);
    }

    private:
    int _width;
    int _height;
};
class Circle : public Surface {
    public:
    Circle() = default;
    Circle(int x, int y, int radius);
    bool checkCollision(int x, int y) const override {
        // sqrt(x² + y²) <= _radius but without sqrt for performance
        return (x*x + y*y) <= _radius*_radius;
    }

    private:
    int _radius;
}; */

struct Fixation;

enum class AOIType {
    Rectangle,
    Circle
};

struct Surface {
    int x;
    int y;
    union {
        struct {
            int width;
            int height;
        };
        struct {
            int radius;
        };
    };
};

class AreaOfInterest {
    public:
    AreaOfInterest() = default;
    AreaOfInterest(AOIType type, Surface surface, std::string name);
    ~AreaOfInterest();

    bool checkCollision(int x, int y) const {
        if (_type == AOIType::Rectangle) {
            return (x > _surface.x) && (x < _surface.x + _surface.width) && (y > _surface.y) && (y < _surface.y + _surface.height);
        }
        else if (_type == AOIType::Circle) {
            // sqrt(x² + y²) <= _radius but without sqrt for performance
            return ((x - _surface.x)*(x - _surface.x) + (y - _surface.y)*(y - _surface.y))
                <= _surface.radius*_surface.radius;
        }
        else {
            throw std::runtime_error("Unknown AOIType in checkCollision");
        }
    }
    double timeOfDwell(const std::vector<Fixation> &fixations) const;
    double timeToFirstFixation(const std::vector<Fixation> &fixations) const;
    double firstFixationDuration(const std::vector<Fixation> &fixations) const;
    double averageFixationDuration(const std::vector<Fixation> &fixations) const;

    std::string getName() const { return _name; }

    private:
    AOIType _type;
    Surface _surface;
    std::string _name;
};


