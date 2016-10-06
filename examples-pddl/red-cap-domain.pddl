(define (domain red-cap)
	(:requirements :strips :typing)
	(:types location actor hero thing)
	(:predicates
		(dead ?x - actor)
		(disguised ?x - actor ?y - actor)
		(carry ?x - actor ?y - thing)
		(positioned-at ?x - actor ?y - location)
		(positioned-at ?x - thing ?y - location)
		(hears ?x - actor ?y -actor)
	)
	(:action move
	 :parameters (?actor - actor ?from ?to - location)
	 :preconditions (and (positioned-at ?actor ?from) (not (positioned-at ?actor ?to)))
	 :effect (and (positioned-at ?actor ?to) (not (positioned-at ?actor ?from)))
	)
	(:action pick-up
	 :parameters (?actor - actor ?object - thing ?location - location)
	 :preconditions (and (positioned-at ?object ?location) (positioned-at ?actor ?location))
	 :effect (and (not (positioned-at ?object ?location) (carry ?actor ?object)))
	)
	(:action eat
	 :parameters (?actor - actor ?subject - actor ?location - location)
	 :preconditions (and (positioned-at ?subject ?location) (positioned-at ?actor ?location))
	 :effect (and (not (positioned-at ?subject ?location)) (carry ?actor ?subject))
	)
	(:action disguise
	 :parameters (?actor - actor ?disguise - actor ?location - location)
	 :preconditions ()
	 :effect (disguised ?actor ?disguise)
	)
	(:action hears
	 :parameters (?actor - actor ?noise - actor ?location - location)
	 :preconditions (and (positioned-at ?actor ?location) (positioned-at ?noise ?location))
	 :effect (hears ?actor ?noise)
	)
	(:action cuts-open
	 :parameters (?actor - actor ?subject - actor ?location - location)
	 :preconditions (and (positioned-at ?actor ?location) (positioned-at ?subject ?location))
	 :postconditions (and (dead ?subject) (not (carry ?subject object)) (positioned-at object ?location)
	)
	(:action skin
	 :parameters (?actor - actor ?subject - actor ?location - location)
	 :preconditions (and (positioned-at ?actor ?location) (positioned-at ?subject ?location) dead(?subject))
	 :postconditions (positioned-at ?wolfskin ?location)
	)
	(:action talk
	 :parameters (?actor - actor ?subject - actor ?location - location)
	 :preconditions (and (positioned-at ?actor ?location) (positioned-at ?subject ?location)
	 :postconditions 
	)
)