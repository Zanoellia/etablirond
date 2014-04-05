
class Car:
  currentNode = 0
  time = 0
  currentDistance = 0
  path = [0]
  graph = None

  def __init__(self, G):
   graph = G

  def getCurrentNode():
    return currentNode

  def getPath():
    return path

  def getDistance():
    return currentDistance

  def getTime():
    return time

  def addMove(node):
    path.append(node)

  def printPath():
    path.reverse()
    print("%d\n", len(path))
    while path:
      print("%d\n", path.pop())

