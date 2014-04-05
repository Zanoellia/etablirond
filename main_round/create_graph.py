#!/usr/bin/python3.3


import networkx as nx
import matplotlib.pyplot as plt

def CreateGraph(numNode, lEdge):
  G = nx.DiGraph()
  i = 0
  while (i < numNode):
    G.add_node(i)
    i += 1
  while (lEdge and not lEdge.empty()):
    (inter1, inter2, direction, cost, length) = lEdge.pop()
    G.add_edge(inter1,inter2, cost=cost, length=length)
    if (direction == 2):
      G.add_edge(inter2, inter1, cost=cost, length=length)
  return G

G = CreateGraph(5, None)
nx.draw(G)
plt.show()
