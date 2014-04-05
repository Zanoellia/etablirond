#!/usr/bin/python3.3

from create_graph import *
from parse import *


def main():
  (lNode, lEdge) = parse()
  G = CreateGraph(lNode, lEdge)
#  nx.draw(G)
#  plt.show()






main()
