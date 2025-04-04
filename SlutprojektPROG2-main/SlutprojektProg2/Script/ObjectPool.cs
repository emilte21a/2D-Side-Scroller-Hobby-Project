
/*
  WARNING... Not my code.
*/

public class ObjectPool<T> where T : new()
{
  // list to hold the objects
  private List<T> objectsInPool = new List<T>();
  //counter keeps track of the number of objects in the pool
  private int counter = 0;
  // max objects allowed in the pool
  private int maxObjectsInPool = 5;

  // returns the number of objects in the pool
  public int GetCount()
  {
    return counter ;
  }

  public T GetObjectFromPool()
  {
    //Declare item
    T objectItem;
    //Check if the pool has any objects
    //If yes, remove the first object and return it
    // also, decrease the count
    if (counter > 0)
    {
      objectItem = objectsInPool[0] ;
      objectsInPool.RemoveAt(0) ;
      counter--;
      return objectItem;
    }
    //If the pool has no objects, create a new one and return it
    else
    {
      T obj = new T();
      return obj;
    }
  }

  // method to return the object to the pool
  // If the counter is less than the max objects allowed, add the object to the pool
  //also, increment counter
  public void ReturnObjToPool(T item)
  {
    if(counter < maxObjectsInPool)
    {
      objectsInPool.Add(item);
      counter++;
    }           
  }
  
}