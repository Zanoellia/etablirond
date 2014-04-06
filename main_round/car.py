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
#    possibleMoves = getNeighboursInfo(G, self.currentNode)
#    maxNodes = []
#  deleteNodes = []
#    maxLength = 0
#    for move in possibleMoves:
#      if (self.time + move[1]['cost'] >= NB_SECS):
#        deleteNodes.append(move)
#
#    for delete in deleteNodes:
#      possibleMoves.remove(delete)
#
#    for move in possibleMoves:
#      if (move[1]['length'] > maxLength):
#          maxNodes = []
#        maxNodes.append(move)
#        maxLength = move[1]['length']
#      if (move[1]['length'] == maxLength):
#        maxNodes.append(move)
#
##    print (maxLength)
#    newList = sorted(maxNodes, key=lambda k: k[1]['cost'])
#    if (newList):
#      newList.reverse()
#      return newList.pop()
#    else:
#      return None

    shts = nx.shortest_path_length(G, self.currentNode, target=None, weight = 'cost')
    print (shts[800])


