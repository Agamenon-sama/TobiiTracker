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
                        else:
                            aoi[pair[0]] = pair[1][1:-3]
            aois.append(areasInFile)
                    
    return aois


def parse():
    """creates persons by iterating over the folder,
    setting the names and calling getAois"""
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
