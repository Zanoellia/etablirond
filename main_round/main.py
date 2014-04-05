#!/usr/bin/python

from create_graph import *
from parse import *
import car

def main():
  (lNode, lEdge) = parse()
  G = CreateGraph(lNode, lEdge)
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
