using System;
using System.Collections.Generic;
using TCDSwift;

using Ident = System.String;

public class IRIdent : IComparable<IRIdent>
{
	private Ident name;
	private IRTuple defsite;
	private HashSet<IRTuple> usesites;

	public IRIdent(Ident name, IRTuple defsite)
	{
		this.name = name;
		this.defsite = defsite;
		this.usesites = new HashSet<IRTuple>();
	}

	public bool Equals(IRIdent other)
	{
		return this.name == other.GetName();
	}

	public int CompareTo(IRIdent other)
	{
		return this.name.CompareTo(other.GetName());
	}

	public Ident GetName()
	{
		return this.name;
	}

	public IRTuple GetDefsite()
	{
		return this.defsite;
	}

	public HashSet<IRTuple> GetUsesites()
	{
		return this.usesites;
	}
}