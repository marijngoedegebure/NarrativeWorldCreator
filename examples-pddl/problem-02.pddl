;; a simple DWR problem with 1 robot and 2 locations
(define (problem dwrpb1)
  (:domain dock-worker-robot-pos)

  (:objects
   r1 - robot
   l1 l2 - location
   k1 k2 - crane
   p1 q1 p2 q2 - pile
   ca cb cc cd ce cf - container)

  (:init
   (adjacent l1 l2)
   (adjacent l2 l1)

   (attached p1 l1)
   (attached q1 l1)
   (attached p2 l2)
   (attached q2 l2)

   (belong k1 l1)
   (belong k2 l2)

   (in ca p1)
   (in cb p1)
   (in cc p1)

   (in cd q1)
   (in ce q1)
   (in cf q1)

   (on ca pallet)
   (on cb ca)
   (on cc cb)

   (on cd pallet)
   (on ce cd)
   (on cf ce)

   (top cc p1)
   (top cf q1)
   (top pallet p2)
   (top pallet q2)

   (atl r1 l1)
   (unloaded r1)
   (free l2)

   (empty k1)
   (empty k2))

;; the task is to move all containers to locations l2
;; ca and cc in pile p2, the rest in q2
  (:goal
    (and (in ca p2)
	        (in cb q2)
	        (in cc p2)
	        (in cd q2)
	        (in ce q2)
	        (in cf q2))))