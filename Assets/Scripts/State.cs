using UnityEngine;
using System.Collections;

public class State {

	string abbreviation;
	string name;
	string capital;

	public State(string abbreviation, string name, string capital) {
		// TODO: Figure out if we need 'this' keyword 

		this.abbreviation = abbreviation;
		this.name = name;
		this.capital = capital;
	}
}
