#!/usr/bin/env python.
# -*- coding: utf-8 -*-

def printCarPath(carPath):
    print len(carPath)
    for i in range(len(carPath)):
        print carPath[i]

def printOutput(cars):
    print C
    for i in range(len(cars)):
        printCarPath(cars[i])

# EXEMPLE D'UTILISATION
# Nombre de voitures C
# en fait inutile dans cet exemple
C = 2

# Liste des intersections visitées par les voitures
# Ici exemple, sinon = résultat de l'algo
car1 = [0]
car2 = [0, 1, 2]

# Liste des voitures
# Pareil, ici exemple en attendant.
cars = [car1, car2]
printOutput(cars)

