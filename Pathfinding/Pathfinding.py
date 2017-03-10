# Taken from old code. There's issues with it - but it'll do for now
# Blame past me for the off naming conventions

def get_route(game_map, start, dest):
    # Performs an A* search to find route to go somewhere
    # Input:
    #   x, y - from self.x and self.y
    #   self.currentMap.Walkable(x,y) - returns if can walk at x,y
    #   dest - (x, y) tuplet of destination
    #
    # Returns a list of (x, y) tuplets
    #
    # This does have a special behaviour that it will list the final square whether or not it can be 
    # walked on - so, it will attack if needed when position is fed to TryMove
    # 
    # From <http://www.policyalmanac.org/games/aStarTutorial.htm>
    # 1) Add the starting square (or node) to the open list.
    # 2) Repeat the following:
    #   a) Look for the lowest F cost square on the open list. We refer to this as the current square.
    #   b) Switch it to the closed list.
    #   c) For each of the 8 squares adjacent to this current square 
    #      If it is not walkable or if it is on the closed list, ignore it. Otherwise do the following.           
    #      If it isn't on the open list, add it to the open list. Make the current square the parent of this square. Record the F, G, and H costs of the square. 
    #      If it is on the open list already, check to see if this path to that square is better, using G cost as the measure. A lower G cost means that this is a better path. If so, change the parent of the square to the current square, and recalculate the G and F scores of the square. If you are keeping your open list sorted by F score, you may need to resort the list to account for the change.
    #   d) Stop when you:
    #      Add the target square to the closed list, in which case the path has been found (see note below), or
    #      Fail to find the target square, and the open list is empty. In this case, there is no path.   
    # 3) Save the path. Working backwards from the target square, go from each square to its parent square until you reach the starting square. That is your path.
    
    # This is to prevent bugs
    if dest == None:
        return None
    
    # ORTH_DISTANCE and DIAG_DISTANCE are for weights of travelling between the cells orthogonally
    # and diagonally respectively. If diagoanal is further in game, then DIAG_DISTANCE should be 14
    # As the distances are the same in mine, they're weighted evenly
    ORTH_DISTANCE = 10
    DIAG_DISTANCE = 10
    
    # Heuristic for calculating h is Manhattan Distance - 
    #   abs(pos.x - dest.x) + abs(pos.y - dest.y)
    
    # OpenLists consists of tuplets with (
    #   [0]: Position.x, 
    #   [1]: Position.y,
    #   [2]: ParentPosition.x, 
    #   [3]: ParentPosition.y,
    #   [4]: g (distance to get here from parent),
    #   [5]: h (heuristic distance to destination) )
    OpenList = [(start[0], start[1], start[0], start[1], 0,
        abs(start[0]-dest[0]) + abs(start[1]-dest[1]))]     
    ClosedList = []                        
    
    Found = None
    
    while (len(OpenList) > 0 and Found == None):                
        # Find entry in OpenList with lowest F score
        # F = G + H                 
        Current = min(OpenList, key=lambda i:i[4]+i[5])         
        OpenList.remove(Current)
        ClosedList.append(Current)
        Active = [(Current[0] - 1,  Current[1],     Current[0], Current[1], Current[4] + ORTH_DISTANCE, abs(Current[0] - 1 - dest[0])   + abs(Current[1] - dest[1])),
            (Current[0] + 1,    Current[1],     Current[0], Current[1], Current[4] + ORTH_DISTANCE, abs(Current[0] + 1 - dest[0])   + abs(Current[1] - dest[1])),
            (Current[0] - 1,    Current[1] - 1, Current[0], Current[1], Current[4] + DIAG_DISTANCE, abs(Current[0] - 1 - dest[0])   + abs(Current[1] - 1 - dest[1])),
            (Current[0] + 1,    Current[1] - 1, Current[0], Current[1], Current[4] + DIAG_DISTANCE, abs(Current[0] + 1 - dest[0])   + abs(Current[1] - 1 - dest[1])),
            (Current[0] - 1,    Current[1] + 1, Current[0], Current[1], Current[4] + DIAG_DISTANCE, abs(Current[0] - 1 - dest[0])   + abs(Current[1] + 1 - dest[1])),
            (Current[0] + 1,    Current[1] + 1, Current[0], Current[1], Current[4] + DIAG_DISTANCE, abs(Current[0] + 1 - dest[0])   + abs(Current[1] + 1 - dest[1])),
            (Current[0],        Current[1] - 1, Current[0], Current[1], Current[4] + ORTH_DISTANCE, abs(Current[0] - dest[0])       + abs(Current[1] - dest[1] - 1)),
            (Current[0],        Current[1] + 1, Current[0], Current[1], Current[4] + ORTH_DISTANCE, abs(Current[0] + dest[0])       + abs(Current[1] - dest[1] + 1))]
        for i in Active:
            # If point not in closed list and is walkable
            # Remove the or (i[0] == dest[0] and i[1] == dest[1] to prevent the special behaviour
            if ((len([j for j in ClosedList if j[0] == i[0] and j[1] == i[1]]) == 0)\
                and (game_map.walkable(i) or (i[0] == dest[0] and i[1] == dest[1]))):
                    
                # Look for point in open List
                Candidate = [j for j in OpenList if j[0] == i[0] and j[1] == i[1]]
                # If point not in open list                 
                if(len(Candidate) == 0):
                    # Add point to the open list
                    OpenList.append(i)
                    if (i[0] == dest[0] and i[1] == dest[1]):
                        Found = i
                else:
                    # Otherwise, check to see if this path to the square is shorter, using G. If so, replace square with current route (changing parent and g) 
                    if Candidate[0][4] > i[4]:
                        OpenList.remove(Candidate[0])
                        OpenList.append(i)
    # If no path found, return empty route
    if Found == None:
        return []
    else:
        # Add path to route             
        CurSquare = Found
        Route = [CurSquare]
        # Iterate until we reach the starting point
        while (not(CurSquare[0] == CurSquare[2] and CurSquare[1] == CurSquare[3])):                 
            CurSquare = [j for j in (OpenList+ClosedList) if j[0] == CurSquare[2] and j[1] == CurSquare[3]][0]
            Route.insert(0, CurSquare)
        return Route