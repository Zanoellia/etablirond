#!/usr/bin/python3.3


import networkx as nx
import matplotlib.pyplot as plt

def CreateGraph(numNode, lEdge):
  G = nx.DiGraph()
  i = 0
  while (i < numNode):
    G.add_node(i)
    i += 1
  while (lEdge):
    (inter1, inter2, direction, cost, length) = lEdge.pop()
    G.add_edge(inter1,inter2, cost=cost, length=length)
    if (direction == 2):
      G.add_edge(inter2, inter1, cost=cost, length=length)
  return G

G = CreateGraph(5, [[0,1,1,5,5],[3,4,4,5,5]])
nx.draw(G)
plt.show()
