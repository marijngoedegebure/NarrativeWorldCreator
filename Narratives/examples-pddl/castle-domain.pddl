(define (domain castle)
	(:requirements :strips :typing)
	(:types 
		character thing place - object
		throneroom - place
		bedroomprincess - place
		guestroom - place
		diningroom - place
		kitchen - place
		prison - place
		corridor - place)
	)
	(:predicates
		(dead ?x - character)
		(has ?character - character ?thing - thing)
		(poisoned ?object - thing)
		(poisoned ?subject - character)
		(loves ?character - character ?subject - character)
		(hidden ?character - character)
		(married ?character1 - character ?character2 - character)
		(at ?object - object ?place - place)
		(on ?object1 - thing ?object - thing ?location - place)
		(dark ?location - place)
		(light ?location - place)
		(narrow ?location - place)
		(small ?location - place)
		(sparse ?location - place)
		(scary ?location - place)
		(around ?object1 - thing ?object2 - thing ?location - place)
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
	 :effect 
	)
	(:action talkmultiple
	 :parameters (?character - character ?subject - character ?subject2 - character ?place - place)
	 :preconditions (and (and ((at ?character ?place) (at ?subject ?place))) (at ?subject2 ?place))
	 :effect 
	)
	(:action fallinlove
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (at ?character ?place) (at ?subject ?place))
	 :effect (loves ?character ?subject)
	)
	(:action hides
	 :parameters (?character - character ?place - place)
	 :preconditions (at ?character ?place)
	 :effect (hidden ?character)
	)
	(:action poisons
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (at ?character ?place) (at ?object ?place))
	 :effect (poisoned ?object)
	)
	(:action swaps
	 :parameters (?character - character ?object1 - thing ?object2 - thing2 ?place - place)
	 :preconditions (and (and (and ((at ?character ?place) (at ?object1 ?place))) (and (at ?object2 ?place) (poisoned (?object1))) (not (poisoned(?object2))))
	 :effect (and (not (poisoned (?object1))) (poisoned (?object2)))
	)
	(:action placeon
	 :parameters (?character - character ?object1 - thing ?object2 - thing ?place - place)
	 :preconditions (and (and ((at ?character ?place) (at ?object1 ?place))) (at ?object2 ?place))
	 :effect (on ?object1 ?object2 ?place)
	)
	(:action drink
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (and ((at ?character ?place) (at ?subject ?place))) poisoned(?object))
	 :effect (poisoned ?character - character)
	)
	(:action dies
	 :parameters (?character - character ?place - place)
	 :preconditions (and (at ?character ?place) (poisoned ?character))
	 :effect (dead ?character)
	)
	(:action marry
	 :parameters (?character1 - character ?character1 - character ?place - place)
	 :preconditions (and (at ?character1 ?place) (at ?character1 ?place))
	 :effect (married ?character1 ?character2)
	)
	(:action give
	 :parameters (?character1 - character ?character2 - character ?object - thing ?place - place)
	 :preconditions (and (and ((at ?character1 ?place) (at ?character2 ?place))) (and (at ?object ?place) (has ?character1 ?object)))
	 :effect (and (not (on ?character1 ?object ?place)) (has ?character2 ?object))
	)
)