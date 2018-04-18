# Dijkstra's Algorithm

This is my implementation of Disjksta's Algorithm implenmented using C#. The main function is a 
specific implementation of the way my Algorithms teacher wanted the program to take input.

# Input

For this program, the main function takes the input as the following in the form of
a text file that the program will prompt the user for.

```
1 2,1 8,2
2 1,1 3,1
3 2,1 4,1
4 3,1 5,1
5 4,1 6,1
6 5,1 7,1
7 6,1 8,1
8 7,1 1,2
```

The first number of each row represents the node name, and each tuple after that
represents the directed edges coming off the node. The first number is the name of
the connected node, and the second number is the "distance" to that node. The names
can be any string, but have to be unique.