#include <iostream>
#include <filesystem>
#include <algorithm>
#include <fstream>

#include <SDL2/SDL.h>
#include <SDL2/SDL_image.h>
#include <SDL2/SDL_ttf.h>

#include "Data.h"
#include "AreaOfInterest.h"
#include "Heatmap.h"
#include "Gazepath.h"

namespace fs = std::filesystem;


bool readData(const std::string &filename, std::vector<DataItem> &items);
bool loadAOIs(const std::string &filename, std::vector<AreaOfInterest> &aois);

int main(int argc, char *argv[]) {
    if (argc != 2) {
        std::cerr << "Usage: " << argv[0] << " <path_to_etd_file>\n";
        return 1;
    }

    SDL_Init(SDL_INIT_VIDEO);
    IMG_Init(IMG_INIT_PNG | IMG_INIT_JPG);
    TTF_Init();

    std::vector<DataItem> items;
    try {
        readData(argv[1], items);
    }
    catch (std::exception &e) {
        std::cerr << "error: " << e.what() << "\n";
        return 2;
    }

    fs::path etdPath(argv[1]);
    auto outputDirName = "output"/etdPath.stem();
    fs::create_directory(outputDirName);

    std::vector<std::string> imageFiles;
    for (const auto &entry : fs::directory_iterator("input/images/")) {
        imageFiles.push_back(entry.path().string());
    }


    SDL_Window *window = SDL_CreateWindow("", 0, 0, 1, 1, SDL_WINDOW_HIDDEN);
    SDL_Renderer *renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);

    for (auto &item : items) {
        Heatmap heatmap(renderer);
        heatmap.render(item);
        heatmap.save(
            outputDirName.string() + "/" + item.fileNumber + "_heatmap.png", // set name of output file
            *std::find_if(imageFiles.begin(), imageFiles.end(), // set name of input file from imageFiles
                // we search for the filename that starts with the fileNumber
                // of the current item
                [=](std::string s) {
                    fs::path p(s);
                    return p.filename().string().substr(0, item.fileNumber.size())  == item.fileNumber;
                }
            )
        );


        Gazepath gazepath(renderer);
        gazepath.render(item);
        gazepath.save(
            outputDirName.string() + "/" + item.fileNumber + "_gazepath.png",
            *std::find_if(imageFiles.begin(), imageFiles.end(),
                [=](std::string s) {
                    fs::path p(s);
                    return p.filename().string().substr(0, item.fileNumber.size())  == item.fileNumber;
                }
            )
        );
        // std::cerr << SDL_GetError() << "\n";

        // std::vector<AreaOfInterest> aois;
        // loadAOIs("input/aois", aois);
        std::fstream aoisOutput(
            outputDirName.string() + "/" + item.fileNumber + "_aois.ppo",
            std::ios::out
        );
        for (auto &aoi : item.aois) {
            aoisOutput << "AOI: " << aoi.getName() << "\n";
            // aoisOutput << "collision: " << aoi.checkCollision(500, 320) << "\n";
            aoisOutput << "Dwell time: " << aoi.timeOfDwell(item.fixations) << "ms\n";
            aoisOutput << "First fixation duration: " << aoi.firstFixationDuration(item.fixations) << "ms\n";
            aoisOutput << "Time of first fixation: " << aoi.timeToFirstFixation(item.fixations) << "ms\n";
            aoisOutput << "Average fixation duration: " << aoi.averageFixationDuration(item.fixations) << "ms\n";
            aoisOutput << "=====\n";
        }
    }

    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(window);

    TTF_Quit();
    IMG_Quit();
    SDL_Quit();
    
    return 0;
}
