#!/usr/bin/python

from create_graph import *
from parse import *
from car import *


def main():
  (lNode, lEdge) = parse()
#  print(lEdge)
  G = CreateGraph(lNode, lEdge)
#  nx.draw(G)
#  plt.show()
  print(visitStreet(G, 1903, 9877))


main()
