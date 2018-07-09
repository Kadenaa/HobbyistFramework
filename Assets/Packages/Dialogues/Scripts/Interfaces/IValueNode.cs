using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IValueNode<T> {
	T GetValue(DialogueController manager);
}