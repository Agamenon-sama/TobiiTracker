#pragma once

#include <SDL2/SDL.h>

#include "heatmap.h"
#include "Data.h"

class Heatmap {
    public:
    Heatmap(SDL_Renderer *renderer);
    ~Heatmap();
    
    void render(const DataItem &item);
    void save(const std::string &filename, const std::string &textureName);

    private:
    heatmap_t *_heatmap;
    uint8_t *_heatmapBuffer; // memory required for the heatmap
    heatmap_stamp_t *_stamp; // shape of the heatmap points

    SDL_Renderer *_renderer; // renderer is not owned it's passed by pointer

    int _width;
    int _height;
};

