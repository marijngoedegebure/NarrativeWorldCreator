(define (problem red-cap)
	(:domain red-cap)
	(:objects 
        basket flowers clothes wolfskin - thing
		grandmother redcap wolf woodman - actor
		motherhouse grandmotherhouse wolfmeetingspot darkwoods woodsmanhouse - location)
    (:init 
    	   (positioned-at redcap motherhouse)
    	   (positioned-at grandmother grandmotherhouse)
    	   (positioned-at wolf wolfmeetingspot)
    	   (positioned-at woodman woodmanshouse)
    	   (carry redcap goodies)
    )
)