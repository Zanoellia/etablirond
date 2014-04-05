#!/usr/bin/python

from create_graph import *
from parse import *
from car import *
from output import *

# GLOBAL VAR FOR INFOS
NB_INTERSECTIONS = 0
NB_STREETS = 0
NB_SECS = 0
NB_CARS = 0
INIT_INTER = 0

def find_a_way(car, graph):
  while 1 == 1:
    neigb = getNeighboursInfo(graph, car.getCurrentNode())
    best = [ -1, -1, [] ]  #id - time - lol
    for n in neigb:
      if best[0] == -1:
        best = [ n[0]['id'], n[1]['cost'], n ]
      try:
        if n[1]['visited'] == 0:
          best = [ n[0]['id'], n[1]['cost'], n ]
      except KeyError:
        pass
    if car.getTime() + best[1] < 4516:
      visitStreet(graph, car.currentNode, best[0])
      car.addMove(best[0], best[1])
    else:
      break


def main():
# init graphe and infos
  (NB_INTERSECTIONS, NB_STREETS, NB_SECS, NB_CARS, INIT_INTER, lNode, lEdge) = parse()
  G = CreateGraph(lNode, lEdge)

# init des voitures
  cars = []
  for i in range(NB_CARS):
    car = Car(G, INIT_INTER)
    find_a_way(car, G)
    cars.append(car)

# algo

#  C = Car();
#  nx.draw(G)
#  plt.show()
#  print(visitStreet(G, 1903, 9877))
  for car in cars:
    while (1):
      nextMove = car.GetNextMove()
      if (nextMove == None):
         break;
      car.AddMove(nextMove)


main()
