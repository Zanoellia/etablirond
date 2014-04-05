#!/usr/bin/python

from create_graph import *
from parse import *
from car import *
from output import *

def main():
# init graphe and infos
  (nb_intersections, nb_streets, nb_secs, nb_cars, init_inter, lNode, lEdge) = parse()
  G = CreateGraph(lNode, lEdge)

# init des voitures
  cars = []
  for i in range(nb_cars):
    car = Car(G)
    cars.append(car)

# algo

#  C = Car();
#  nx.draw(G)
#  plt.show()
#  print(visitStreet(G, 1903, 9877))
#  print(getNeighboursInfo(G, 9877))

# output
  printOutput(nb_cars, cars)



main()
