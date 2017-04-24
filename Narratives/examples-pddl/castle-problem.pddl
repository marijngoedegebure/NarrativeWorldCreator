(define (problem castle)
	(:domain castle)
	(:objects 
        poison cupking cupprince cupprincess table chair throne - thing
		prince princess king maiden - character
		throneroom1 - throneroom 
        bedroomprincess1 - bedroomprincess
        guestroom1 - guestroom
        diningroom1 - diningroom
        kitchen1 - kitchen
        prison1 - prison
        corridor1 - corridor)
    (:init 
            (at prince throneroom1)
            (at princess throneroom1)
            (at king throneroom1)
            (at maiden guestroom1)
            (at poison bedroomprincess1)
            (at cupking kitchen1)
            (at cupprince kitchen1)
            (at cupprincess kitchen1)
            (dark guestroom1)
            (light diningroom1)
            (narrow corridor1)
            (scary corridor1)
            (sparse corridor1)
            (small guestroom1)
            (at table diningroom1)
            (at chair diningroom1)
            (at chair diningroom1)
            (at chair diningroom1)
            (at throne throneroom1)
            (at chair throneroom1)
            (around chair table diningroom1)
    )
)