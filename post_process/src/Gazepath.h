#pragma once

#include <SDL2/SDL.h>
#include <SDL2/SDL_ttf.h>

#include "Data.h"

class Gazepath {
    public:
    Gazepath(SDL_Renderer *renderer);
    ~Gazepath();

    void render(const DataItem &item);
    void save(const std::string &filename, const std::string &textureName);

    private:
    SDL_Renderer *_renderer; // renderer is not owned it's passed by pointer
    SDL_Texture *_texture;
    TTF_Font *_font;

    int _width;
    int _height;
};

