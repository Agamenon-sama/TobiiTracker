# TobiiTracker

This project was my undergraduate thesis (final project).

It involves a software that shows images and texts and the software tracks the eye movements using the tobii eyex tracker and records those data. A second software post processes those data and creates heatmaps and other representations.

## Dependencies

- SDL2
- SDL2_images
- SDL2_ttf

For the client, you also need to install the tobii driver to use the eye tracker.

## Build

For the client, you need to build the program using visual studio.

For the post processing software, use the make file.

The first software can only be built and used on Windows because of dependencies. The post processing software was designed for Linux although it might work on Windows too but you might not be able to use the makefile.

## License

The software is licensed under the MIT license.

Additional dependencies are included in this repository including [libheatmap](https://github.com/lucasb-eyer/libheatmap) licensed under MIT, [stb](https://github.com/nothings/stb) licensed under unlicense and [SDL2-CS](https://github.com/flibitijibibo/SDL2-CS) licensed under some BSD-like license.

You can find the licenses of these dependencies in the source files and in the previously linked repositories.
