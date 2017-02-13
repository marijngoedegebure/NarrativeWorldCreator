(define (domain red-cap)
	(:requirements :strips :typing)
	(:types 
		character thing place - object
		hero - character)
	(:predicates
		(dead ?x - character)
		(has ?character - character ?thing - thing)
		(at ?character - character ?place - place)
		(at ?thing - thing ?place - place)
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
	 :preconditions (and (at ?character ?from) (not (at ?character ?to)))
	 :effect (and (at ?character ?to) (not (at ?character ?from)))
	)
	(:action pickup
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (at ?object ?place) (at ?character ?place))
	 :effect (and (not (at ?object ?place) (has ?character ?object)))
	)
	(:action talk
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (at ?character ?place) (at ?subject ?place))
	 :postconditions 
	)
	(:action talkmultiple
	 :parameters (?character - character ?subject - character ?subject2 - character ?place - place)
	 :preconditions (and (and ((at ?character ?place) (at ?subject ?place))) (at ?subject2 ?place))
	 :postconditions 
	)
	(:action fallinlove
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (at ?character ?place) (at ?subject ?place))
	 :postconditions (loves ?character ?subject)
	)
	(:action hides
	 :parameters (?character - character ?place - place)
	 :preconditions (at ?character ?place)
	 :postconditions (hidden ?character)
	)
	(:action poisons
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (at ?character ?place) (at ?object ?place))
	 :postconditions (poisoned ?object)
	)
	(:action swaps
	 :parameters (?character - character ?object1 - thing ?object2 - thing2 ?place - place)
	 :preconditions (and (and (and ((at ?character ?place) (at ?object1 ?place))) (and (at ?object2 ?place) (poisoned (?object1))) (not (poisoned(?object2))))
	 :postconditions (and (not (poisoned (?object1))) (poisoned (?object2)))
	)
	(:action placeon
	 :parameters (?character - character ?object1 - thing ?object2 -thing ?place - place)
	 :preconditions (and (and ((at ?character ?place) (at ?object1 ?place))) (at ?object2 ?place))
	 :postconditions (on ?object1 ?object2)
	)
	(:action drinks
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (and ((at ?character ?place) (at ?subject ?place))) poisoned(?object))
	 :postconditions (poisoned ?character - character)
	)
	(:action dies
	 :parameters (?character - character ?place - place)
	 :preconditions (and (at ?character ?place) (poisoned ?character))
	 :postconditions (dead ?character)
	)
	(:action marry
	 :parameters (?character1 - character ?character1 - character ?place - place)
	 :preconditions (and (at ?character1 ?place) (at ?character1 ?place))
	 :postconditions (married ?character1 ?character2)
	)
)