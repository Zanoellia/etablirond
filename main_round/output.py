#!/usr/bin/env python.
# -*- coding: utf-8 -*-

from car import *

def printOutput(C, cars):
    print C
    for i in range(C):
        cars[i].printPath()

# EXEMPLE D'UTILISATION
# Nombre de voitures C
# en fait inutile dans cet exemple
C = 2

# Liste des intersections visitées par les voitures
# Ici exemple, sinon = résultat de l'algo
car1 = Car(None)
car1.path = [0]
car2 = Car(None)
car2.path = [0, 1, 2]

# Liste des voitures
# Pareil, ici exemple en attendant.
cars = [car1, car2]
printOutput(C, cars)

