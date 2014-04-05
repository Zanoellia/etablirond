#!/usr/bin/python3.3

import networkx as nx

def CreateGraph(lNode, lEdge):
  G = nx.DiGraph()
  i = 0
  while (lNode):
    (lati, longi) = lNode.pop()
    G.add_node(i, id=i, lati=lati, longi=longi)
    i += 1
  while (lEdge):
    (inter1, inter2, direction, cost, length) = lEdge.pop()
    G.add_edge(inter1,inter2, cost=cost, length=length, visited=0)
    if (direction == 2):
      G.add_edge(inter2, inter1, cost=cost, length=length)
  return G


def visitStreet(G, startNode, destNode):
  if (G[startNode][destNode]):
    G[startNode][destNode]['visited'] = 1
    try:
      G[destNode][startNode]['visited'] = 1
    except KeyError:
      pass
    return [G[startNode][destNode]['cost'], G[startNode][destNode]['length']]
  else:
    return None


def getNeighboursInfo(G, startNode):
  ret = []
  nodes = G.neighbors(startNode)
  for node in nodes:
    ret.append([ G.node[node], G[startNode][node]])
  return ret



