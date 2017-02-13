(define (domain red-cap)
	(:requirements :strips :typing)
	(:types 
		character thing place - object
		hero - character)
	(:predicates
		(dead ?x - character)
		(has ?character - character ?thing - thing)
		(atChar ?character - character ?place - place)
		(atThing ?thing - thing ?place - place)
		(poisoned ?object - thing)
		(poisoned ?subject - character)
		(loves ?character - character ?subject - character)
		(hidden ?character - character)
		(on ?object1 - thing ?object - thing)
		(married ?character1 - character ?character2 - character)
		(dark ?location - place)
		(light ?location - place)
		(narrow ?location - place)
		(small ?location - place)
		(sparse ?location - place)
		(scary ?location - place)
		(around ?object1 - thing ?object2 - thing)
	)
	(:action move
	 :parameters (?character - character ?from ?to - place)
	 :preconditions (and (atChar ?character ?from) (not (atChar ?character ?to)))
	 :effect (and (atChar ?character ?to) (not (atChar ?character ?from)))
	)
	(:action pickup
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (atThing ?object ?place) (atThing ?character ?place))
	 :effect (and (not (atThing ?object ?place) (has ?character ?object)))
	)
	(:action talk
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (atChar ?character ?place) (atChar ?subject ?place))
	 :postconditions 
	)
	(:action talkmultiple
	 :parameters (?character - character ?subject - character ?subject2 - character ?place - place)
	 :preconditions (and (and ((atChar ?character ?place) (atChar ?subject ?place))) (atChar ?subject2 ?place))
	 :postconditions 
	)
	(:action fallinlove
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (atChar ?character ?place) (atChar ?subject ?place))
	 :postconditions (loves ?character ?subject)
	)
	(:action hides
	 :parameters (?character - character ?place - place)
	 :preconditions (atChar ?character ?place)
	 :postconditions (hidden ?character)
	)
	(:action poisons
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (atChar ?character ?place) (atThing ?object ?place))
	 :postconditions (poisoned ?object)
	)
	(:action swaps
	 :parameters (?character - character ?object1 - thing ?object2 - thing2 ?place - place)
	 :preconditions (and (and (and ((atChar ?character ?place) (atThing ?object1 ?place))) (and (atThing ?object2 ?place) (poisoned (?object1))) (not (poisoned(?object2))))
	 :postconditions (and (not (poisoned (?object1))) (poisoned (?object2)))
	)
	(:action placeon
	 :parameters (?character - character ?object1 - thing ?object2 -thing ?place - place)
	 :preconditions (and (and ((atChar ?character ?place) (atThing ?object1 ?place))) (atThing ?object2 ?place))
	 :postconditions (on ?object1 ?object2)
	)
	(:action drink
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (and ((atChar ?character ?place) (atThing ?subject ?place))) poisoned(?object))
	 :postconditions (poisoned ?character - character)
	)
	(:action dies
	 :parameters (?character - character ?place - place)
	 :preconditions (and (atChar ?character ?place) (poisoned ?character))
	 :postconditions (dead ?character)
	)
	(:action marry
	 :parameters (?character1 - character ?character1 - character ?place - place)
	 :preconditions (and (atChar ?character1 ?place) (atChar ?character1 ?place))
	 :postconditions (married ?character1 ?character2)
	)
	(:action give
	 :parameters (?character1 - character ?character2 - character ?object - thing ?place - place)
	 :preconditions (and (and ((atChar ?character1 ?place) (atChar ?character2 ?place))) (and (atThing ?object ?place) (has ?character1 ?object)))
	 :postconditions (and (not (on ?character1 ?object)) (has ?character2 ?object))
	)
)