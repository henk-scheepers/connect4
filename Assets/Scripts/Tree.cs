using System;
using System.Collections.Generic;

public class Tree<T> where T: IComparable{
    T data;
    List<Tree<T>> children = new List<Tree<T>>();

    public Tree(T data){
        this.data = data;
    }

    public T Data{
        get{ return data; }
    }

    public void Graft(Tree<T> tree){
        children.Add(tree);
    }

    public Tree<T> Prune(T value){
        foreach(Tree<T> child in children){
            if(value.CompareTo(child.Data) == 0){
                children.Remove(child);
                return child;
            }else{
                Tree<T> prunedChild = child.Prune(value);
                if(prunedChild != null){
                    return prunedChild;
                }
            }
        }

        return null;
    }

    public Tree<T> Find(T value){
        if(data.CompareTo(value) == 0){
            return this;
        }

        foreach(Tree<T> child in children){
            Tree<T> foundChild = child.Find(value);
            if(foundChild != null){
                return foundChild;
            }
        }

        return null;
    }
}