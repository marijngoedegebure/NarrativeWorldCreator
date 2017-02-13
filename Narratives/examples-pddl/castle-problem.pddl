(define (problem castle)
	(:domain castle)
	(:objects 
        poison cupking cupprince cupprincess table chair - thing
		prince princess king maiden - character
		throneroom bedroomprincess guestroom diningroom kitchen prison corridor - place)
    (:init 
            (atChar prince throneroom)
            (atChar princess throneroom)
            (atChar king throneroom)
            (atChar maiden guestroom)
            (atThing poison bedroomprincess)
            (atThing cupking kitchen)
            (atThing cupprince kitchen)
            (atThing cupprincess kitchen)
            (dark guestroom)
            (light diningroom)
            (narrow corridor)
            (scary corridor)
            (sparse corridor)
            (small guestroom)
            (atThing table diningroom)
            (atThing chair diningroom)
            (atThing chair diningroom)
            (atThing chair diningroom)
            (atThing throne throneroom)
            (atThing chair throneroom)
            (around chair table)
    )
)