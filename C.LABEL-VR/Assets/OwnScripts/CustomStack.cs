using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CustomStack<T>
{
    private List<T> stack = new List<T>();

    public int Count { get { return stack.Count; } }

    public IEnumerator<T> GetEnumerator()
    {
        return stack.GetEnumerator();
    }

    public void Push(T item)
    {
        stack.Add(item);
    }

    public T Pop()
    {
        if (stack.Count > 0)
        {
            T temp = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return temp;
        }
        else
            return default(T);
    }

    public T Peek()
    {
        if (stack.Count > 0)
        {
            return stack[stack.Count - 1];
        }
        else
        {
            return default(T);
        }
    }

    public void Remove(int itemAtPosition)
    {
        stack.RemoveAt(itemAtPosition);
    }

    public void Remove(T item)
    {
        stack.Remove(item);
    }

    public bool Contains(T item)
    {
        return stack.Contains(item);
    }

    public T ElementAt(int index)
    {
        return stack.ElementAt(index);
    }
}
