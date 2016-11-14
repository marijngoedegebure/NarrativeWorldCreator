(define (problem red-cap)
	(:domain red-cap)
	(:objects 
        basket flowers clothes wolfskin - thing
		grandmother redcap wolf woodman - character
		motherhouse insidegrandmotherhouse outsidegrandmotherhouse wolfmeetingspot woodmanshouse flowerpickuplocation - place)
    (:init 
            (at redcap motherhouse)
            (at grandmother insidegrandmotherhouse)
            (at wolf wolfmeetingspot)
            (at woodman woodmanshouse)
            (has grandmother clothes)
            (has redcap goodies)
    )
)