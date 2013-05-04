using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nephrite.Metamodel
{
	public interface IMMObjectTransition
	{
		DateTime CreateDate { get; set; }
		string Comment { get; set; }
		bool IsCurrent { get; set; }
		bool IsLast { get; set; }
		int SeqNo { get; set; }
		int ParentID { get; set; }
		Guid ParentGUID { get; set; }
		int SubjectID { get; set; }
		int WorkflowID { get; set; }
		int ActivityID { get; set; }
		int TransitionID { get; set; }
	}
}
