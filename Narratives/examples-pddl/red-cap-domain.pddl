(define (domain red-cap)
	(:requirements :strips :typing)
	(:types 
		character thing place - object
		hero - character)
	(:predicates
		(dead ?x - character)
		(disguised ?x - character ?y - character)
		(has ?character - character ?thing - thing)
		(at ?character - character ?place - place)
		(at ?thing - thing ?place - place)
		(hears ?x - character ?y -character)
	)
	(:action move
	 :parameters (?character - character ?from ?to - place)
	 :preconditions (and (at ?character ?from) (not (at ?character ?to)))
	 :effect (and (at ?character ?to) (not (at ?character ?from)))
	)
	(:action pick-up
	 :parameters (?character - character ?object - thing ?place - place)
	 :preconditions (and (at ?object ?place) (at ?character ?place))
	 :effect (and (not (at ?object ?place) (has ?character ?object)))
	)
	(:action eat
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (at ?subject ?place) (at ?character ?place))
	 :effect (and (not (at ?subject ?place)) (has ?character ?subject))
	)
	(:action disguise
	 :parameters (?character - character ?disguise - character ?place - place)
	 :preconditions ()
	 :effect (disguised ?character ?disguise)
	)
	(:action hears
	 :parameters (?character - character ?noise - character ?place - place)
	 :preconditions (and (at ?character ?place) (at ?noise ?place))
	 :effect (hears ?character ?noise)
	)
	(:action cuts-open
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (at ?character ?place) (at ?subject ?place))
	 :postconditions (and (dead ?subject) (not (has ?subject object)) (at object ?place)
	)
	(:action skin
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (at ?character ?place) (at ?subject ?place) dead(?subject))
	 :postconditions (at ?wolfskin ?place)
	)
	(:action talk
	 :parameters (?character - character ?subject - character ?place - place)
	 :preconditions (and (at ?character ?place) (at ?subject ?place)
	 :postconditions 
	)
)