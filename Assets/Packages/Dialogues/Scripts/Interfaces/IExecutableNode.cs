using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IExecutableNode {
	void Execute(DialogueController manager);
}