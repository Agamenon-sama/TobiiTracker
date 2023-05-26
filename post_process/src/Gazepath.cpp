#include "Gazepath.h"
#include "stb_image.h"
#include "stb_image_write.h"

#include <SDL2/SDL_image.h>

#include <stdexcept>
#include <iostream>


Gazepath::Gazepath(SDL_Renderer *renderer) {
    _renderer = renderer;
    _width = 1366;
    _height = 768;

    _texture = SDL_CreateTexture(_renderer, SDL_PIXELFORMAT_ABGR8888, SDL_TEXTUREACCESS_TARGET, _width, _height);
    SDL_SetTextureBlendMode(_texture, SDL_BLENDMODE_BLEND);

    const std::string fontPath = "res/fonts/Ubuntu-R.ttf";
    _font = TTF_OpenFont(fontPath.c_str(), 22);
    if (!_font) {
        throw std::runtime_error("Can't open font at " + fontPath);
    }
}

Gazepath::~Gazepath() {
    TTF_CloseFont(_font);
    _font = nullptr;
    SDL_DestroyTexture(_texture);
    _texture = nullptr;
}

void Gazepath::render(const DataItem &item) {
    SDL_SetRenderTarget(_renderer, _texture);
    SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 0);
    SDL_RenderClear(_renderer);
    uint32_t fixationNumber = 0; // initilize to 0 and set to 1 on first long fixation
    for (const auto &fixation : item.fixations) {
        if (fixation.data.size() > 8) {
            fixationNumber++;

            // computing coordinate
            int x = fixation.begin.x;
            x += fixation.end.x;
            int y = fixation.begin.y;
            y += fixation.end.y;
            for (const auto &data : fixation.data) {
                x += data.x;
                y += data.y;
            }
            x /= (fixation.data.size() + 2);
            y /= (fixation.data.size() + 2);
            
            // drawing square
            SDL_SetRenderDrawColor(_renderer, 0x30, 0xc2, 0xe3, 0x90);
            SDL_Rect rect = {
                .x = x - 25,
                .y = y - 25,
                .w = 50,
                .h = 50
            };
            SDL_RenderFillRect(_renderer, &rect);

            // drawing number of fixation sequence on square
            SDL_Color numberColor = {
                .r = 0xff,
                .g = 0xff,
                .b = 0xff,
                .a = 0xdd,
            };
            auto numberTextSurface = TTF_RenderUTF8_Blended(_font, std::to_string(fixationNumber).c_str(), numberColor);
            if (!numberTextSurface) {
                continue; // if can't render text, just move to the next 
            }
            int numberHeight = numberTextSurface->h;
            int numberWidth = numberTextSurface->w;
            auto numberText = SDL_CreateTextureFromSurface(_renderer, numberTextSurface);
            SDL_Rect numberRect = {
                .x = x - numberWidth / 2,
                .y = y - numberHeight / 2,
                .w = numberWidth,
                .h = numberHeight
            };
            SDL_RenderCopy(_renderer, numberText, nullptr, &numberRect);
            SDL_DestroyTexture(numberText);
            SDL_FreeSurface(numberTextSurface);
        }
    }
    SDL_SetRenderTarget(_renderer, nullptr);
}

void Gazepath::save(const std::string &filename, const std::string &textureName) {
    //  load input texture
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
    // SDL_FreeSurface(bgSurface);
    SDL_UpdateTexture(bgTexture, nullptr, image, 4 * width);
    SDL_SetTextureBlendMode(bgTexture, SDL_BLENDMODE_BLEND);
    delete image;

    // blend loaded and generated textures
    SDL_SetRenderTarget(_renderer, bgTexture);
    SDL_RenderCopy(_renderer, _texture, nullptr, nullptr);

    // save file
    SDL_Surface *surface = SDL_CreateRGBSurface(0, _width, _height, 32,
        0x000000ff, // these masks only work for little endian
        0x0000ff00,
        0x00ff0000,
        0xff000000);
    SDL_RenderReadPixels(_renderer, nullptr, surface->format->format, surface->pixels, surface->pitch);
    if (IMG_SavePNG(surface, filename.c_str())) {
        SDL_FreeSurface(surface);
        throw std::runtime_error("Can't save gazepath for image: " + textureName);
    }
    SDL_FreeSurface(surface);
    SDL_DestroyTexture(bgTexture);
    SDL_SetRenderTarget(_renderer, nullptr);
}

