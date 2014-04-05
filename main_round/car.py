from create_graph import *

NB_SECS = 54000

class Car:
  def __init__(self, G, start):
   self.currentNode = start
   self.time = 0
   self.currentDistance = 0
   self.path = [ start ]
   self.graph = G

  def getCurrentNode():
    return self.currentNode

  def getCurrentNode(self):
    return self.currentNode

  def getPath(self):
    return self.path

  def getDistance(self):
    return self.currentDistance

  def getTime(self):
    return self.time

  def addMove(self, G, node, time):
    self.path.append(node)
    self.time = self.time + time
    visitStreet(G, self.currentNode, node)
    self.currentNode = node

  def printPath(self):
    print (len(self.path))
    for i in range(len(self.path)):
        print (self.path[i])

  def getNextMove(self, G):

    shts = nx.shortest_path_length(G, self.currentNode, target=None, weight = 'cost')
    for key, value in shts.items():
      if (value >= NB_SECS):
        del shts[key]
    if (not shts):
      return None

    shts2 = nx.shortest_path_length(G, self.currentNode, target=None, weight = 'length')
    sorted_shts2 = sorted(shts2.items(), key=lambda x:x[1])
    gotoNode = sorted_shts2.pop()

    path = nx.dijkstra_path(G, self.currentNode, gotoNode[0], weight='cost')

    path.pop(0)
    for pos in path:
      self.path.append(pos)
      visitStreet(G, self.currentNode, pos)
#     self.addMove(G, pos, 0)
      self.currentNode = pos
    self.time += shts[gotoNode[0]]

    return 1



