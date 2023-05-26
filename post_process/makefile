.PHONY : all clean

APP := out
CC := gcc
CXX := g++
CXX_FLAGS := -std=c++17 -Wall -g # -fsanitize=address
LINK_FLAGS := -lSDL2 -lSDL2_image -lSDL2_ttf

SRC_DIR := ./src
BUILD_DIR := ./build

OBJ := $(BUILD_DIR)/main.o $(BUILD_DIR)/heatmap.o $(BUILD_DIR)/stb_image.o $(BUILD_DIR)/Loader.o \
	$(BUILD_DIR)/Heatmap.o $(BUILD_DIR)/Gazepath.o $(BUILD_DIR)/AreaOfInterest.o

all: output

output: $(OBJ)
	$(CXX) -o $(APP) $(OBJ) $(CXX_FLAGS) $(LINK_FLAGS)

$(BUILD_DIR)/%.o: $(SRC_DIR)/%.c
	$(CC) $(CXX_FLAGS) $(LINK_FLAGS) -c $< -o $@

$(BUILD_DIR)/%.o: $(SRC_DIR)/%.cpp
	$(CXX) $(CXX_FLAGS) $(LINK_FLAGS) -c $< -o $@

clean:
	rm $(BUILD_DIR)/*
	rm -r "output/*"

run: all
	@echo Running app
	./$(APP) "input/etds/ADAMSZ.etd"
