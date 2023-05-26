#include "Heatmap.h"

#include "stb_image.h"
#include "stb_image_write.h"

#include <SDL2/SDL_image.h>

#include <stdexcept>
#include <iostream>

Heatmap::Heatmap(SDL_Renderer *renderer) {
    _renderer = renderer;
    _width = 1366; // temp
    _height = 768; // temp
    _heatmap = heatmap_new(_width, _height);
    _heatmapBuffer = new uint8_t[_width * _height * 4];
    _stamp = heatmap_stamp_gen(15); // create stamp of radius 15
}

Heatmap::~Heatmap() {
    heatmap_stamp_free(_stamp);
    delete [] _heatmapBuffer;
    heatmap_free(_heatmap);
}

void Heatmap::render(const DataItem &item) {
    // add the points to the heatmap buffer
    for (const auto &fixation : item.fixations) {
        if (fixation.begin.x != 0 && fixation.begin.y != 0)
            heatmap_add_point_with_stamp(_heatmap, fixation.begin.x, fixation.begin.y, _stamp);
        for (const auto &data : fixation.data) {
            if (data.x != 0 && data.y != 0)
                heatmap_add_point_with_stamp(_heatmap, data.x, data.y, _stamp);
        }
        if (fixation.end.x != 0 && fixation.end.y != 0)
            heatmap_add_point_with_stamp(_heatmap, fixation.end.x, fixation.end.y, _stamp);
    }

    heatmap_render_default_to(_heatmap, _heatmapBuffer);
}

void Heatmap::save(const std::string &filename, const std::string &textureName) {
    // load background texture
    int width, height, channels;
    uint8_t *image = stbi_load(textureName.c_str(), &width, &height, &channels, 4);
    if (!image) {
        throw std::runtime_error("stbi can't load image: " + textureName);
    }
    auto bgTexture = SDL_CreateTexture(_renderer, SDL_PIXELFORMAT_ABGR8888, SDL_TEXTUREACCESS_TARGET, width, height);
    if (!bgTexture) {
        delete image;
        throw std::runtime_error("Can't create texture for: " + textureName);
    }
    SDL_UpdateTexture(bgTexture, nullptr, image, 4 * width);
    SDL_SetTextureBlendMode(bgTexture, SDL_BLENDMODE_BLEND);
    delete image;

    // create heatmap texture
    auto heatmapTexture = SDL_CreateTexture(_renderer, SDL_PIXELFORMAT_ABGR8888, SDL_TEXTUREACCESS_STREAMING, _width, _height);
    if (!heatmapTexture) {
        throw std::runtime_error("Can't create texture for texture: " + textureName);
    }
    SDL_SetTextureBlendMode(heatmapTexture, SDL_BLENDMODE_BLEND);
    SDL_UpdateTexture(heatmapTexture, nullptr, _heatmapBuffer, 4 * _width);

    // blend both textures
    SDL_SetRenderTarget(_renderer, bgTexture);
    SDL_RenderCopy(_renderer, heatmapTexture, nullptr, nullptr);

    // save file
    SDL_Surface *surface = SDL_CreateRGBSurface(0, _width, _height, 32,
        0x000000ff, // these masks only work for little endian
        0x0000ff00,
        0x00ff0000,
        0xff000000);
    SDL_RenderReadPixels(_renderer, nullptr, surface->format->format, surface->pixels, surface->pitch);
    if (IMG_SavePNG(surface, filename.c_str())) {
        SDL_FreeSurface(surface);
        throw std::runtime_error("Can't save heatmap for image: " + textureName);
    }
    SDL_FreeSurface(surface);
    SDL_SetRenderTarget(_renderer, nullptr);
    SDL_DestroyTexture(bgTexture);
}
