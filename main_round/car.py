from create_graph import *

class Car:
  def __init__(self, G):
   self.currentNode = 0
   self.time = 0
   self.currentDistance = 0
   self.path = [0]
   self.graph = G

  def getCurrentNode(self):
    return self.currentNode

  def getPath(self):
    return self.path

  def getDistance(self):
    return self.currentDistance

  def getTime(self):
    return self.time

  def addMove(self, node):
    self.path.append(node)

  def printPath(self):
    print len(self.path)
    for i in range(len(self.path)):
        print self.path[i]

  def getNextMove(G):
    possibleMoves = getNeighboursInfo(G, currentNode)
    maxNodes = []
    maxLength = 0
    for move in possibleMoves:
      if (self.time + move[1]['cost'] >= NB_SECS):
        possibleMoves.remove(move)
        continue
      if (move[1]['length'] > maxLength):
        maxNodes = []
        maxNodes.append(move)
      if (move[1]['length'] == maxLength):
        maxNodes.append(move)

    newlist = sorted(maxNodes, key=lambda k: k[1]['cost'])
    return maxNodes.pop()


