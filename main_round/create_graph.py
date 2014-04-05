#!/usr/bin/python3.3


import networkx as nx
import matplotlib.pyplot as plt

def CreateGraph(lNode, lEdge):
  G = nx.DiGraph()
  i = 0
  while (lNode):
    (lati, longi) = lNode.pop()
    G.add_node(i, lati=lati, longi=longi)
    i += 1
  while (lEdge):
    (inter1, inter2, direction, cost, length) = lEdge.pop()
    G.add_edge(inter1,inter2, cost=cost, length=length)
    if (direction == 2):
      G.add_edge(inter2, inter1, cost=cost, length=length)
  return G

