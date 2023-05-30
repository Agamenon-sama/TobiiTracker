#! /usr/bin/env python3

from pathlib import Path

class Person:
    def __init__(self, name) -> None:
        self.name = name
        self.areas = []

def getAois(folder : Path):
    """iterate over the .ppo file to set the aois of a person"""
    aois = [] # list of dictionaries. each dict has a name and a list of aois
    for f in folder.iterdir():
        if f.is_file() and f.suffix == ".ppo" and f.stat().st_size > 0:
            areasInFile = {}
            areasInFile["name"] = f.stem
            areasInFile["aois"] = []
            with f.open() as file:
                aoi = {} # holds 1 aoi block from 1 file
                for line in file:
                    if line == "=====\n": # when we hit the end of an aoi
                        areasInFile["aois"].append(aoi) # we push it to the list
                        aoi = {} # might not need it but I'll keep it
                    else: # else we get aoi info
                        # the key is the name of the aoi option which is what is before the :
                        # the value is what is after but without the space and the "ms"
                        pair = line.split(':')
                        if pair[0] == "AOI":
                            aoi[pair[0]] = pair[1][1:-1]
                        elif pair[0] == "Average fixation duration":
                            aoi[pair[0]] = float(pair[1][1:-3])
                        else:
                            aoi[pair[0]] = int(pair[1][1:-3])
            aois.append(areasInFile)
                    
    return aois

def interestingAois(persons : list, filename : str) -> dict:
    """
        Calculates the average dwell time for the aois of a certain file.
        filename is the name of the file without the .ppo extension.
    """
    dwellTimes = []

    # create tuples of aoi names and their dwell times for all testers
    for p in persons:
        for file in p.areas:
            if file["name"] == filename: # search for the desired file
                for aoi in file["aois"]:
                    dwellTimes.append((aoi["AOI"], aoi["Dwell time"]))

    # accum is a dictionary that holds:
    # keys: the names of the aois
    # values: the dwell time (by the end, it will hold the average)
    accum = {}

    # set the keys from dwellTimes in the dict
    # and set their values to 0 for accumulating
    for elem in dwellTimes:
        accum[elem[0]] = 0

    # accumulating the values from dwellTimes
    for elem in dwellTimes:
        accum[elem[0]] += elem[1]

    # calculate averages
    for elem in accum:
        accum[elem] /= len(persons)

    return accum

def parse():
    """
        creates list of persons by iterating over the output folder,
        setting the names of the persons and the information of the eye tracking
        test on every file
    """
    persons = []

    rootFolder = Path("output/")
    for f in rootFolder.iterdir():
        if f.is_dir():
            dirName = f.name[:len(rootFolder.name)]
            person = Person(dirName)
            person.areas = getAois(f)
            persons.append(person)
    
    return persons

if __name__ == "__main__":
    persons = parse()
    for p in persons:
        print(p.name)
        for area in p.areas:
            print(area["name"])
            print(area["aois"])
