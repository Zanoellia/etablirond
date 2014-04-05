
class Car:
  def __init__(self, G, start):
   self.currentNode = start
   self.time = 0
   self.currentDistance = 0
   self.path = [ start ]
   self.graph = G

  def getCurrentNode():
    return currentNode

  def getCurrentNode(self):
    return self.currentNode

  def getPath(self):
    return self.path

  def getDistance(self):
    return self.currentDistance

  def getTime(self):
    return self.time

  def addMove(self, node, time):
    self.path.append(node)
    self.time = self.time + time

  def printPath(self):
    print (len(self.path))
    for i in range(len(self.path)):
        print (self.path[i])
