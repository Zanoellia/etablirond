#!/usr/bin/python

from create_graph import *
from parse import *
import car


def main():
  (lNode, lEdge) = parse()
  G = CreateGraph(lNode, lEdge)
  C = Car();
#  nx.draw(G)
#  plt.show()
#  print(visitStreet(G, 1903, 9877))


main()
