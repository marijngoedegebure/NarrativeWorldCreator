(define (plan castle)
	(:problem castle)
	(:steps
		(talkmultiple prince princess king throneroom)
		(fallinlove princess prince throneroom)
		(fallinlove prince princess throneroom)
		(hides maiden guestroom)
		(move prince throneroom guestroom)
		(talk princess king throneroom)
		(move princess throneroom corridor)
		(move princess corridor guestroom)
		(talk princess prince guestroom)
		(move princess guestroom bedroomprincess)
		(pickup princess poison bedroomprincess)
		(move princess bedroomprincess guestroom)
		(talk prince princess guestroom)
		(give princess prince poison guestroom)
		(move prince guestroom diningroom)
		(move princess guestroom diningroom)
		(move king throneroom diningroom)
		(move maiden guestroom kitchen)
		(hides maiden kitchen)
		(talkmultiple prince princess king diningroom)
		(move prince diningroom kitchen)
		(poisons cupking poison kitchen)
		(move prince kitchen diningroom)
		(swaps maiden cupking cupprince kitchen)
		(pickup maiden cupprince kitchen)
		(move maiden kitchen diningroom)
		(placeon maiden cupprince table diningroom)
		(drink prince cupprince diningroom)
		(dies prince diningroom)
		(talk princess king diningroom)
		(move princess diningroom prison)
		(marry king maiden diningroom)
	)
)