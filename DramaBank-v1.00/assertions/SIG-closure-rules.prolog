% FollowedByTransitive allows for transitive temporal relationships.
followedByTransitive(A,B) :-
	followedByFlat(A,B).

followedByTransitive(A,B) :-
	setof((A,B,C), (
	followedByFlat(A,C),  % "consume" a base hop
	\+ modifier(C),% bridge must not be a modifier
	followedByTransitive(C,B)), Result),
	member((A,B,C),Result).


followedByOrSimultaneous(A,B) :-
	followedByTransitive(A,B).
followedByOrSimultaneous(A,B) :-
	A=B.

wouldCauseTransitive(A,B,_) :-
	wouldCause(A,B).
wouldCauseTransitive(A,B,_) :-
	providesFor(A,B).
wouldCauseTransitive(A,B,IMPLIED) :-
	wouldCause(A,IMPLIED),
	wouldCauseTransitive(IMPLIED,B,_).
wouldCauseTransitive(A,B,IMPLIED) :-
	wouldPrevent(A,IMPLIED),
	wouldPreventTransitive(IMPLIED,B,_).
wouldCauseTransitive(A,B) :-
	setof((A,B,C), (
	wouldCauseTransitive(A,B,C)), Answers),
	member((A,B,C), Answers).

% Flatten the various arcs for positive and 
% negative actualiztion status transitions.
actualizesFlat(A,B) :-
	actualizes(A,B).
actualizesFlat(A,B) :-
	interpretedAs(A,B).
actualizesFlat(A,B) :-
	implies(A,B).
ceasesFlat(A,B) :-
	ceases(A,B).

% "Actualizes" and "ceases" must be placed explicitly,
% except for Core Goals which are assumed to be affected.
% Causes a beneficial factor.
actualizesTransitive(A,B,_) :-	
	actualizesFlat(A,B).
actualizesTransitive(A,B,IMPLIED) :-	
	actualizesFlat(A,IMPLIED),
	providesFor(IMPLIED,B).
% Stops a damaging factor.
actualizesTransitive(A,B,IMPLIED) :-	
	ceasesFlat(A,IMPLIED),
	damages(IMPLIED,B).
actualizesTransitive(A,B,IMPLIED) :-
	actualizesFlat(A, IMPLIED),
	equivalentOf(IMPLIED, B),
	A\==B.
actualizesTransitive(A,B,IMPLIED) :-
	ceasesFlat(A, IMPLIED),
	inverseOf(IMPLIED, B).


% Excludes frames themselves from actualization arcs;
% content is what matters.
actualizesTransitiveContent(A,B) :-
	actualizesTransitive(A,B,_),
	\+ outermostFrame(B).
actualizesTransitive(A,B) :-
	setof((A,B,IMPLIED), (
	actualizesTransitive(A,B,IMPLIED)), Answers),
	member((A,B,IMPLIED), Answers).

% Ceases a damaging factor.
ceasesTransitive(A,B,_) :-
	ceasesFlat(A,B).
ceasesTransitive(A,B,IMPLIED) :-	
	actualizesFlat(A,IMPLIED),
	damages(IMPLIED,B).
% Ceases a beneficial factor.
ceasesTransitive(A,B,IMPLIED) :-	
	ceasesFlat(A,IMPLIED),
	providesFor(IMPLIED,B).
ceasesTransitive(A,B,IMPLIED) :-
	ceasesFlat(A, IMPLIED),
	equivalentOf(IMPLIED, B).
ceasesTransitive(A,B,IMPLIED) :-
	actualizesFlat(A, IMPLIED),
	inverseOf(IMPLIED, B).

ceasesTransitiveContent(A,B) :-
	ceasesTransitive(A,B,_),
	\+ outermostFrame(B).
ceasesTransitive(A,B) :-
	ceasesTransitive(A,B,_).



% These are used to go "through" frames to content.  

% An agent declares an intention to do some goal content by
% actualizing its goal frame.
declaresIntention(ACTION, GOAL, AGENT) :-
	setof((ACTION,GOAL,AGENT,GOALBOX,IMPLIED), (
	actualizesTransitive(ACTION,GOALBOX,IMPLIED),
	interpNodeIn(GOAL,GOALBOX),
	goalBox(GOALBOX),
	agent(GOALBOX, AGENT)), Result),
	member((ACTION, GOAL, AGENT,GOALBOX,IMPLIED), Result).

% An agent declares an intention to do some goal content
% by attempting to cause it.
declaresIntention(ACTION, GOAL, AGENT) :-
	setof((ACTION,GOAL,AGENT,GOAL,IMPLIED1,IMPLIED2), (
	attemptToCauseTransitive(ACTION,GOAL,IMPLIED1,IMPLIED2),
	agent(ACTION, AGENT)), Result),
	member((ACTION, GOAL, AGENT, GOAL, IMPLIED1, IMPLIED2), Result).

% (In case we want to enforce that the attempted
% goal content must be in a frame by the same agent:)
	%interpNodeIn(GOAL,GOALBOX),
	%goalBox(GOALBOX),
	%agent(GOALBOX,AGENT1),
	%agent(ACTION,AGENT2),
	%AGENT1=AGENT2.

% An agent declares belief in some content.
declaresBelief(ACTION, CONTENT, AGENT) :-
	actualizesTransitive(ACTION, BELIEFBOX,_),
	interpNodeIn(CONTENT, BELIEFBOX),
	beliefBox(BELIEFBOX),
	agent(BELIEFBOX, AGENT).
declaresBelief(ACTION, CONTENT, AGENT, BELIEFBOX) :-
	actualizesTransitive(ACTION, BELIEFBOX,_),
	interpNodeIn(CONTENT, BELIEFBOX),
	beliefBox(BELIEFBOX),
	agent(BELIEFBOX, AGENT).

% An agent ceases its intention to do some goal content by
% ceasing its goal frame.
ceasesIntention(ACTION,GOAL) :-
	ceasesTransitive(ACTION, GOALBOX,_),
	interpNodeIn(GOAL, GOALBOX),
	goalBox(GOALBOX).

% An agent ceases its intention to do some goal content
% by attempting to prevent it.
ceasesIntention(ACTION,GOAL) :-
	attemptToPreventTransitive(ACTION,GOAL,_,_).

% An agent ceases belief in some content.
ceasesBelief(ACTION, CONTENT, AGENT, BELIEFBOX) :-
	ceasesTransitive(ACTION, BELIEFBOX,_),
	interpNodeIn(CONTENT, BELIEFBOX),
	beliefBox(BELIEFBOX),
	agent(BELIEFBOX, AGENT).






preconditionForTransitive(A,B,_) :-
	providesFor(A,B).
preconditionForTransitive(A,B,_) :-
	preconditionForFlat(A,B,_).
% wouldCause is essentially a supertype of preconditionFor.
% wouldCause means necessary and sufficient; preconditionFor means necessary.
preconditionForTransitive(A,B,IMPLIED) :-
	wouldCauseTransitive(A,B,IMPLIED).
preconditionForTransitive(A,B,IMPLIED) :-
	preconditionForFlat(A,B,IMPLIED).
preconditionForTransitive(A,B,IMPLIED) :-
	preconditionForFlat(A,IMPLIED,_),
	preconditionForTransitive(IMPLIED,B,_).
preconditionForTransitive(A,B,IMPLIED) :-
	wouldCauseTransitive(A,IMPLIED,_),
	preconditionForTransitive(IMPLIED,B,_).
preconditionForTransitive(A,B,IMPLIED) :-
	preconditionForFlat(A,IMPLIED,_),
	wouldCauseTransitive(IMPLIED,B,_).
% If X would prevent Y which is preventing Z, X is a precondition for Z
preconditionForTransitive(A,B,IMPLIED) :-
	wouldPreventTransitive(A,IMPLIED,_),
	wouldPreventTransitive(IMPLIED,B,_).
% If X is an agency box surrounding a node which is a precondition for Y,
% X is itself a precondition for Y
preconditionForTransitive(A,B,IMPLIED) :-
	interpNodeInTransitive(IMPLIED,A,_),
	preconditionForTransitive(IMPLIED,B,_).
preconditionForTransitive(A,B) :-
	setof((A,B,C), (
	preconditionForTransitive(A,B,C)), Answers),
	member((A,B,C), Answers).

preconditionAgainstTransitive(A,B,IMPLIED) :-
	wouldPreventTransitive(A,B,IMPLIED).
preconditionAgainstTransitive(A,B,IMPLIED) :-
	wouldPreventTransitive(A,IMPLIED,_),
	preconditionForTransitive(IMPLIED,B,_).


%%% CAUSLITY %%%
% Identify when two timeline propositions have a causal relationship.
% A causes B
causalTimelinePropositions(A,B,IMPLIED1,IMPLIED2) :-
	followedByOrSimultaneous(A,B),
	actualizesFlat(A,IMPLIED1),
	actualizesFlat(B,IMPLIED2),
	because(IMPLIED2,IMPLIED1).
causalTimelinePropositions(A,B,IMPLIED1,IMPLIED2) :-
	followedByOrSimultaneous(A,B),
	actualizesFlat(A,IMPLIED1),
	preconditionForFlat(IMPLIED1,IMPLIED2,_),
	actualizesFlat(B,IMPLIED2).
causalTimelinePropositions(A,B,IMPLIED1,IMPLIED2) :-
	followedByOrSimultaneous(A,B),
	actualizesFlat(A,IMPLIED1),
	wouldPrevent(IMPLIED1,IMPLIED2),
	ceasesFlat(B,IMPLIED2).
causalTimelinePropositions(A,B,IMPLIED1,IMPLIED2) :-
	followedByOrSimultaneous(A,B),
	ceasesFlat(A,IMPLIED1),
	wouldPrevent(IMPLIED1,IMPLIED2),
	actualizesFlat(B,IMPLIED2).
causalTimelinePropositions(A,B,IMPLIED1,IMPLIED2) :-
	followedByOrSimultaneous(A,B),
	ceasesFlat(A,IMPLIED1),
	preconditionForFlat(IMPLIED1,IMPLIED2,_),
	actualizesFlat(B,IMPLIED2).
causalTimelinePropositions(A,B) :-
	causalTimelinePropositions(A,B,_,_).
causalTimelinePropositionsTransitive(A,B) :-
	causalTimelinePropositions(A,IMPLIED,_,_),
	causalTimelinePropositions(IMPLIED,B,_,_).


wouldPreventTransitive(A,B,_) :-
	wouldPrevent(A,B).
wouldPreventTransitive(A,B,_) :-
	damages(A,B).
% Preventing an X that would cause a Y effectively would prevent Y.
wouldPreventTransitive(A,B,IMPLIED) :-
	wouldPrevent(A,IMPLIED),
	wouldCauseTransitive(IMPLIED,B,_).
% Causing an X that would prevent a Y effectively would prevent Y. 
wouldPreventTransitive(A,B,IMPLIED) :-
	wouldCauseTransitive(A,IMPLIED,_),
	wouldPreventTransitive(IMPLIED,B,_).
wouldPreventTransitive(A,B) :-
	wouldPreventTransitive(A,B,_).


attemptToCauseTransitive(ACTION,GOAL,_,_) :-
	attemptToCause(ACTION,GOAL).
% If an agent does something that they believe would eventually lead
% to a goal, they are attempting to do that goal.
attemptToCauseTransitive(ACTION,GOAL,IMPLIED_SUBGOAL,IMPLIED_PRECONDITION) :-
	attemptToCause(ACTION,IMPLIED_SUBGOAL),
	agent(ACTION,AGENT),
	wouldCauseTransitive(IMPLIED_SUBGOAL,GOAL,IMPLIED_PRECONDITION),
	partOfAgentBelief(GOAL,AGENT).
attemptToCauseTransitive(ACTION,GOAL,IMPLIED_SUBGOAL,IMPLIED_PRECONDITION) :-
	attemptToCause(ACTION,IMPLIED_SUBGOAL),
	agent(ACTION,AGENT),
	preconditionForTransitive(IMPLIED_SUBGOAL,GOAL,IMPLIED_PRECONDITION),
	partOfAgentBelief(GOAL,AGENT).
attemptToCauseTransitive(ACTION, GOAL) :-
	setof((ACTION, GOAL, IMPLIED1, IMPLIED2), (
	attemptToCauseTransitive(ACTION, GOAL, IMPLIED1, IMPLIED2)), Result),
	member((ACTION, GOAL, IMPLIED1, IMPLIED2), Result).


attemptToPreventTransitive(ACTION,GOAL,_,_) :-
	attemptToPrevent(ACTION,GOAL).
attemptToPreventTransitive(ACTION,GOAL,IMPLIED_SUBGOAL,IMPLIED_PRECONDITION) :-
	attemptToPrevent(ACTION,IMPLIED_SUBGOAL),
	agent(ACTION,AGENT),
	preconditionForTransitive(IMPLIED_SUBGOAL,GOAL,IMPLIED_PRECONDITION),
	groundTruthOrOfAgent(GOAL,AGENT).
attemptToPreventTransitive(ACTION,GOAL,IMPLIED_SUBGOAL,IMPLIED_PRECONDITION) :-
	attemptToCause(ACTION,IMPLIED_SUBGOAL),
	agent(ACTION,AGENT),
	preconditionAgainstTransitive(IMPLIED_SUBGOAL,GOAL,IMPLIED_PRECONDITION),
	groundTruthOrOfAgent(GOAL,AGENT).
attemptToPreventTransitive(A,B) :-
	attemptToPreventTransitive(A,B,_,_).



modifier(A) :-
	modifies(A,_).

% followedByFlat integrates modifiers into the followedBy temporal chain.
followedByFlat(A,B) :- 
	modifies(B,C),
	followedBy(A,C).
followedByFlat(A,B) :- 
	modifies(A,C),
	followedBy(C,B).
followedByFlat(A,B) :-
	followedBy(A,B).

% preconditionForFlat integrates goals into their host agency boxes 
preconditionForFlat(X,Y,_) :-
	preconditionFor(X,Y).
% wouldCause is essentially a supertype of preconditionFor.
% wouldCause means necessary and sufficient; preconditionFor means necessary.
preconditionForFlat(X,Y,_) :-
	wouldCause(X,Y).
preconditionForFlat(X,Y,IMPLIED_GOALBOX) :-
	interpNodeIn(Y,IMPLIED_GOALBOX),
	preconditionFor(X,IMPLIED_GOALBOX).


% A node exists in any agency box by some agent (goal, belief, obligation)
partOfAgentBelief(GOAL,AGENT) :-
	interpNodeInTransitive(GOAL, SUPERGOAL,_),
	agent(SUPERGOAL, AGENT).

interpNodeInTransitive(X,Y,_) :-
	interpNodeIn(X,Y).
interpNodeInTransitive(X,Z,IMPLIED) :-
	interpNodeIn(X,IMPLIED),
	interpNodeInTransitive(IMPLIED,Z,_).





%%%% SUPPORTING %%%%
verbalize(A,B,C,A_TEXT,B_TEXT,C_TEXT) :-
	sourceText(A, A_TEXT),
	sourceText(B, B_TEXT),
	sourceText(C, C_TEXT).

verbalize(A,B,A_TEXT,B_TEXT) :-
	sourceText(A, A_TEXT),
	sourceText(B, B_TEXT).

verbalize(A) :-
	write(A).


goalBox(A) :-
	type(A,B),
	B==goalBox.
goalBox(A) :-
	type(A,B),
	B==obligationBox.


beliefBox(A) :-
	type(A,B),
	B==beliefBox.

coreGoal(A) :-
	type(A,B),
	B==coreGoal.

sameAgent(A, B) :-
	agent(A, A_AGENT),
	agent(B, B_AGENT),
	A_AGENT==B_AGENT.

sameAgentCoreGoal(A, B) :-
	coreGoal(A),
	coreGoal(B),
	agent(A, A_AGENT),
	agent(B, B_AGENT),
	A_AGENT==B_AGENT.

frame(A) :-
	goalBox(A).
frame(A) :-
	beliefBox(A).

goalOrBeliefBox(A) :-
	goalBox(A).
goalOrBeliefBox(A) :-
	beliefBox(A).

outermostFrame(A) :-
	frame(A),
	\+ interpNodeIn(A,_).

groundTruth(A) :-
	\+ interpNodeIn(A,_).


goalOfAgent(GOAL, AGENT) :-
	interpNodeIn(GOAL, GOALBOX),
	agent(GOALBOX, AGENT).

intention(P, I) :-
	attemptToPreventTransitive(P, I).
intention(P, I) :-
	attemptToCauseTransitive(P, I).


groundTruthOrOfAgent(I, AGENT) :-
	interpNodeIn(I, BOX),
	agent(BOX, AGENT).
groundTruthOrOfAgent(I, AGENT) :-
	groundTruth(I),
	AGENT==AGENT.


declaresExpectationToCause(P, BELIEF, EXPECTATION, AGENT) :-
	actualizesTransitive(P, BELIEFBOX, _),
	interpNodeIn(BELIEF, BELIEFBOX),
	goalOrBeliefBox(BELIEFBOX),
	agent(BELIEFBOX, AGENT),
	groundTruthOrOfAgent(EXPECTATION, AGENT),
	wouldCauseTransitive(BELIEF, EXPECTATION).

declaresExpectationToCause(P, BELIEF, EXPECTATION, AGENT) :-
	declaresIntention(P, BELIEF, AGENT),
	interpNodeIn(BELIEF, BELIEFBOX),
	goalOrBeliefBox(BELIEFBOX),
	agent(BELIEFBOX, AGENT),
	groundTruthOrOfAgent(EXPECTATION, AGENT),
	wouldCauseTransitive(BELIEF, EXPECTATION).


declaresExpectationToPrevent(P, BELIEF, EXPECTATION, AGENT) :-
	actualizesTransitive(P, BELIEFBOX, _),
	interpNodeIn(BELIEF, BELIEFBOX),
	goalOrBeliefBox(BELIEFBOX),
	agent(BELIEFBOX, AGENT),
	groundTruthOrOfAgent(EXPECTATION, AGENT),
	wouldPreventTransitive(BELIEF, EXPECTATION).

declaresExpectationToPrevent(P, BELIEF, EXPECTATION, AGENT) :-
	declaresIntention(P, BELIEF, AGENT),
	interpNodeIn(BELIEF, BELIEFBOX),
	goalOrBeliefBox(BELIEFBOX),
	agent(BELIEFBOX, AGENT),
	groundTruthOrOfAgent(EXPECTATION, AGENT),
	wouldPreventTransitive(BELIEF, EXPECTATION).


actualizesAid(P, I, COREGOAL) :-
	actualizesTransitive(P, I),
	wouldCauseTransitive(I, COREGOAL),
	coreGoal(COREGOAL).
actualizesAid(P, I, COREGOAL) :-
	ceasesTransitive(P, I),
	wouldPreventTransitive(I, COREGOAL),
	coreGoal(COREGOAL).
actualizesAid(P, COREGOAL) :-
	actualizesAid(P, _, COREGOAL),
	coreGoal(COREGOAL).
actualizesAid(P, COREGOAL) :-
	actualizesTransitive(P, COREGOAL),
	coreGoal(COREGOAL).

actualizesHarm(P, I, COREGOAL) :-
	actualizesTransitive(P, I),
	wouldPreventTransitive(I, COREGOAL),
	coreGoal(COREGOAL).
actualizesHarm(P, I, COREGOAL) :-
	ceasesTransitive(P, I),
	preconditionForTransitive(I, COREGOAL),
	coreGoal(COREGOAL).
actualizesHarm(P, COREGOAL) :-
	actualizesHarm(P, _, COREGOAL),
	coreGoal(COREGOAL).
actualizesHarm(P, COREGOAL) :-
	ceasesTransitive(P, COREGOAL),
	coreGoal(COREGOAL).

wouldAid(I, COREGOAL, AGENT) :-
	preconditionForTransitive(I, COREGOAL),
	coreGoal(COREGOAL),
	agent(COREGOAL, AGENT).

wouldHarm(I, COREGOAL, AGENT) :-
	wouldPreventTransitive(I, COREGOAL),
	coreGoal(COREGOAL),
	agent(COREGOAL, AGENT).



%%%% FUNCTIONS FOR TEXT CLASSIFICATION EXPERIMENT %%%%

actualizesOrImplies(A,B) :-
	actualizesFlat(A,B).
actualizesOrImplies(A,B) :-
	implies(A,B).


textSpanActualizesOrCeasesGoalBox(S, BEGIN, END, GOALBOX) :-
	actualizesOrImplies(P, GOALBOX),
	goalBox(GOALBOX),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpanActualizesOrCeasesGoalBox(S, BEGIN, END, GOALBOX) :-
	ceasesTransitive(P, GOALBOX),
	goalBox(GOALBOX),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpanActualizesOrCeasesBeliefBox(S, BEGIN, END, BELIEFBOX) :-
	actualizesOrImplies(P, BELIEFBOX),
	beliefBox(BELIEFBOX),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpanActualizesOrCeasesBeliefBox(S, BEGIN, END, BELIEFBOX) :-
	ceasesTransitive(P, BELIEFBOX),
	beliefBox(BELIEFBOX),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpanActualizesAid(S, BEGIN, END, AGENT) :-
	actualizesFlat(P, TRIGGER),
	providesFor(TRIGGER, COREGOAL),
	agent(COREGOAL, AGENT),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpanActualizesHarm(S, BEGIN, END, AGENT) :-
	actualizesFlat(P, TRIGGER),
	damages(TRIGGER, COREGOAL),
	agent(COREGOAL, AGENT),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpanGoalDirectedAction(S, BEGIN, END, AGENT) :-
	attemptToCauseTransitive(P, _),
	agent(P, AGENT),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpanGoalDirectedAction(S, BEGIN, END, AGENT) :-
	attemptToPreventTransitive(P, _),
	agent(P, AGENT),
	sourceText(P, S),
	sourceTextBeginOffset(P, BEGIN),
	sourceTextEndOffset(P, END).

textSpansCausallyRelated(S1, BEGIN1, END1, S2, BEGIN2, END2) :-
	causalTimelinePropositions(P1, P2),
	sourceText(P1, S1),
	sourceText(P2, S2),
	sourceTextBeginOffset(P1, BEGIN1),
	sourceTextEndOffset(P1, END1),
	sourceTextBeginOffset(P2, BEGIN2),
	sourceTextEndOffset(P2, END2).


%%%% STATIC SIG PATTERNS %%%%


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.2: Affectual Status Transitions %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

gain(P,COREGOAL) :-
	coreGoal(COREGOAL),
	actualizesTransitive(P,COREGOAL).
gain(P,COREGOAL) :-
	gain(P, _, COREGOAL).
gain(P,I,COREGOAL) :-
	setof((P,I,COREGOAL),(
	coreGoal(COREGOAL),
	actualizesTransitive(P,I),
	\+ outermostFrame(I),
	preconditionForTransitive(I, COREGOAL)), Result),
	member((P,I,COREGOAL),Result).
gain(P,I,COREGOAL) :-
	setof((P,I,COREGOAL), (
	coreGoal(COREGOAL),
	ceasesTransitive(P,I),
	\+ outermostFrame(I),
	wouldPreventTransitive(I, COREGOAL)), Result),
	member((P,I,COREGOAL),Result).

loss(P,COREGOAL) :-
	coreGoal(COREGOAL),
	ceasesTransitive(P,COREGOAL).
loss(P,COREGOAL) :-
	loss(P, _, COREGOAL).

loss(P,I,COREGOAL) :-
	setof((P,I,COREGOAL),(
	coreGoal(COREGOAL),
	ceasesTransitive(P,I),
	\+ outermostFrame(I),
	preconditionForTransitive(I, COREGOAL)), Result),
	member((P,I,COREGOAL), Result).
loss(P,I,COREGOAL) :-
	setof((P,I,COREGOAL), (
	coreGoal(COREGOAL),
	actualizesTransitive(P,I),
	\+ outermostFrame(I),
	wouldPreventTransitive(I, COREGOAL)), Result),
	member((P,I,COREGOAL), Result).


negativeResolution(P1, P2, COREGOAL) :-
	actualizesTransitive(P1, COREGOAL),
	coreGoal(COREGOAL),
	ceasesTransitive(P2, COREGOAL),
	followedByTransitive(P1, P2).


positiveResolution(P1, P2, COREGOAL) :-
	ceasesTransitive(P1, COREGOAL),
	coreGoal(COREGOAL),
	actualizesTransitive(P2, COREGOAL),
	followedByTransitive(P1, P2).


complexPositive(P, COREGOAL1, COREGOAL2, ROUTE1, ROUTE2) :-
	coreGoal(COREGOAL1),
	coreGoal(COREGOAL2),
	actualizesTransitive(P, COREGOAL1, ROUTE1),
	actualizesTransitive(P, COREGOAL2, ROUTE2),
	ROUTE1 \== ROUTE2,
	\+ preconditionForTransitive(ROUTE1, ROUTE2),
	\+ preconditionForTransitive(ROUTE2, ROUTE1),
	sameAgentCoreGoal(COREGOAL1,COREGOAL2).

complexNegative(P, COREGOAL1, COREGOAL2, ROUTE1, ROUTE2) :-
	coreGoal(COREGOAL1),
	coreGoal(COREGOAL2),
	ceasesTransitive(P, COREGOAL1, ROUTE1),
	ceasesTransitive(P, COREGOAL2, ROUTE2),
	ROUTE1 \== ROUTE2,
	\+ preconditionForTransitive(ROUTE1, ROUTE2),
	\+ preconditionForTransitive(ROUTE2, ROUTE1),
	sameAgentCoreGoal(COREGOAL1,COREGOAL2).


hiddenBlessing(P1, P2, COREGOAL1, COREGOAL2) :-
	loss(P1, COREGOAL1),
	causalTimelinePropositions(P1, P2),
	gain(P2, COREGOAL2),
	sameAgentCoreGoal(COREGOAL1, COREGOAL2).


positiveTradeoff(P1, P2, COREGOAL1, COREGOAL2) :-
	loss(P2, COREGOAL1),
	sameAgentCoreGoal(COREGOAL1, COREGOAL2),
	gain(P2, COREGOAL2),
	gain(P1, COREGOAL1),
	followedByTransitive(P1, P2).


negativeTradeoff(P1, P2, COREGOAL1, COREGOAL2) :-
	gain(P2, COREGOAL1),
	sameAgentCoreGoal(COREGOAL1, COREGOAL2),
	loss(P2, COREGOAL2),
	loss(P1, COREGOAL1),
	followedByTransitive(P1, P2).


mixedBlessing(P1, P2, COREGOAL1, COREGOAL2) :-
	gain(P1, COREGOAL1),
	sameAgentCoreGoal(COREGOAL1, COREGOAL2),
	loss(P2, COREGOAL2).


promise(P, POTENTIAL, PROMISE, COREGOAL) :-
	coreGoal(COREGOAL),
	actualizesTransitive(P, PROMISE),
	wouldCauseTransitive(PROMISE, COREGOAL, POTENTIAL),
	POTENTIAL\==COREGOAL.

threat(P, POTENTIAL, THREAT, COREGOAL) :-
	coreGoal(COREGOAL),
	actualizesTransitive(P, THREAT),
	wouldPreventTransitive(THREAT, COREGOAL, POTENTIAL),
	POTENTIAL\==COREGOAL.

promiseFulfilled(P1, P2, POTENTIAL, PROMISE, COREGOAL) :-
	promise(P1, POTENTIAL, PROMISE, COREGOAL),
	actualizesTransitive(P2, POTENTIAL),
	followedByTransitive(P1, P2).

threatFulfilled(P1, P2, POTENTIAL, THREAT, COREGOAL) :-
	threat(P1, POTENTIAL, THREAT, COREGOAL),
	actualizesTransitive(P2, POTENTIAL),
	followedByTransitive(P1, P2).

promiseBroken(P1, P2, POTENTIAL, PROMISE, COREGOAL) :-
	promise(P1, POTENTIAL, PROMISE, COREGOAL),
	ceasesTransitive(P2, POTENTIAL),
	followedByTransitive(P1, P2).
	
threatAvoided(P1, P2, POTENTIAL, THREAT, COREGOAL) :-
	threat(P1, POTENTIAL, THREAT, COREGOAL),
	ceasesTransitive(P2, POTENTIAL),
	followedByTransitive(P1, P2).


promiseOrThreat(P1, POTENTIAL, PROMISE, COREGOAL) :-
	promise(P1, POTENTIAL, PROMISE, COREGOAL).

promiseOrThreat(P1, POTENTIAL, PROMISE, COREGOAL) :-
	threat(P1, POTENTIAL, PROMISE, COREGOAL).



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.3: Examples of Chaining %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

partialResolution(P1, P2, COREGOAL1, COREGOAL2) :-
	complexNegative(P1, COREGOAL1, COREGOAL2,_,_),
	positiveResolution(P1, P2, COREGOAL1).

compoundedTransition(P1, P2, P3, I, COREGOAL) :-
	loss(P1, I, COREGOAL),
	followedByTransitive(P1, P2),
	gain(P2, I, COREGOAL),
	followedByTransitive(P2, P3),
	loss(P3, I, COREGOAL).

compoundedTransition(P1, P2, P3, I, COREGOAL) :-
	gain(P1, I, COREGOAL),
	followedByTransitive(P1, P2),
	loss(P2, I, COREGOAL),
	followedByTransitive(P2, P3),
	gain(P3, I, COREGOAL).


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.4: Single-Agent Goals and Plans %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

goalDeclared(P, GOAL, GOALBOX) :-
	setof((P, GOAL, GOALBOX), (
	actualizesTransitive(P,GOALBOX,_),
	interpNodeIn(GOAL,GOALBOX),
	goalBox(GOALBOX)), Answers),
	member((P, GOAL, GOALBOX),Answers).


% We can also assume that an agent has a goal if it attempts to cause
% its content directly without having explicitly actualized the goal
% frame.
goalDeclared(P, GOAL, GOALBOX) :-
	attemptToCauseTransitive(P,GOAL,_,_),
	interpNodeIn(GOAL, GOALBOX).
	
desireToAid(P, GOAL, GOALBOX, COREGOAL) :-
	coreGoal(COREGOAL),
	actualizesTransitive(P,GOALBOX,_),
	interpNodeIn(GOAL,GOALBOX),
	goalBox(GOALBOX),
	wouldCauseTransitive(GOAL, COREGOAL).

% We can also assume that an agent has a goal if it attempts to cause
% its content directly without having explicitly actualized the goal
% frame.
desireToAid(P, GOAL, GOALBOX, COREGOAL) :-
	coreGoal(COREGOAL),
	attemptToCauseTransitive(P,COREGOAL,GOAL,_),
	interpNodeIn(GOAL, GOALBOX).

desireToHarm(P, GOAL, GOALBOX, COREGOAL) :-
	coreGoal(COREGOAL),
	actualizesTransitive(P,GOALBOX,_),
	interpNodeIn(GOAL,GOALBOX),
	goalBox(GOALBOX),
	wouldPreventTransitive(GOAL, COREGOAL).

desireToHarm(P, GOAL, GOALBOX, COREGOAL) :-
	coreGoal(COREGOAL),
	attemptToPreventTransitive(P,COREGOAL,GOAL,_),
	interpNodeIn(GOAL, GOALBOX).


explicitMotivation(P1, P2, GOALBOX) :-
	causalTimelinePropositions(P1, P2),
	actualizesTransitive(P2, GOALBOX),
	goalBox(GOALBOX).


problem(P1, P2, PROBLEM, COREGOAL, PLAN, GOALBOX) :-
	actualizesFlat(P1, PROBLEM),
	wouldPreventTransitive(PROBLEM, COREGOAL),
	coreGoal(COREGOAL),
	followedByTransitive(P1, P2),
	actualizesTransitive(P2, GOALBOX),
	interpNodeIn(PLAN, GOALBOX),
	wouldPreventTransitive(PLAN, PROBLEM).


changeOfMind(P1, P2, GOAL, GOALBOX) :-
	goalDeclared(P1, GOAL, GOALBOX),
	ceasesTransitive(P2, GOALBOX),
	followedByTransitive(P1, P2).


goalEnablement(P1, P2, GOAL, GOALBOX, ENABLER) :-
	setof((P1, GOAL, GOALBOX), (
	goalDeclared(P1, GOAL, GOALBOX)), Answers),
	member((P1, GOAL, GOALBOX), Answers),
	preconditionForTransitive(ENABLER, GOAL),
	actualizesTransitive(P2, ENABLER),
	followedByTransitive(P1, P2).


goalObstacle(P1, P2, GOAL, GOALBOX, CEASED_PRECONDITION) :-
	setof((P1, GOAL, GOALBOX), (
	goalDeclared(P1, GOAL, GOALBOX)), Answers),
	member((P1, GOAL, GOALBOX), Answers),
	preconditionForTransitive(CEASED_PRECONDITION,GOAL),
	ceasesTransitive(P2, CEASED_PRECONDITION),
	followedByTransitive(P1, P2).


goalSuccessExpected(P1, P2, GOAL, GOALBOX, ENABLER) :-
	goalDeclared(P1, GOAL, GOALBOX),
	actualizesTransitive(P2, ENABLER),
	wouldCauseTransitive(ENABLER,GOAL),
	followedByTransitive(P1, P2).


goalFailureExpected(P1, P2, GOAL, GOALBOX, DISABLER) :-
	goalDeclared(P1, GOAL, GOALBOX),
	actualizesTransitive(P2, DISABLER),
	wouldPreventTransitive(DISABLER,GOAL),
	followedByTransitive(P1, P2).


goalFailureExpected(P1, P2, GOAL, GOALBOX, DISABLER) :-
	goalDeclared(P1, GOAL, GOALBOX),
	ceasesTransitive(P2, DISABLER),
	preconditionForTransitive(DISABLER, GOAL),
	followedByTransitive(P1, P2).


goalAvoidance(P1, P2, TRIGGER, GOAL, GOALBOX) :-
	followedByTransitive(P1, P2),
	actualizesTransitive(P1, TRIGGER),
	wouldCauseTransitive(TRIGGER, GOALBOX),
	ceasesTransitive(P2, GOALBOX),
	interpNodeIn(GOAL, GOALBOX).


goalPreemption(P1, P2, AGENT, PLAN, GOALBOX, PREEMPTED) :-
	goalDeclared(P1, PLAN, GOALBOX),
	wouldPreventTransitive(PLAN, PREEMPTED),
	goalBox(PREEMPTED),
	agent(PREEMPTED, AGENT),
	agent(GOALBOX, AGENT),
	agent(P2, AGENT),
	actualizesTransitive(P2, PLAN),
	followedByTransitive(P1, P2).


perseverance(P1, P2, GOAL, GOALBOX) :-
	setof((P1, GOAL, GOALBOX), (
	goalDeclared(P1, GOAL, GOALBOX)), Result),
	member((P1, GOAL, GOALBOX), Result),
	attemptToCauseTransitive(P2, GOAL),
	followedByTransitive(P1, P2).


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.5: Single-Agent Goal Outcomes %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

success(P1, P2, GOAL, GOALBOX) :-
	followedByTransitive(P1, P2),
	goalDeclared(P1, GOAL, GOALBOX),
	actualizesTransitive(P2, GOAL).


failure(P1, P2, GOAL, GOALBOX) :-
	followedByTransitive(P1, P2),
	goalDeclared(P1, GOAL, GOALBOX),
	ceasesTransitive(P2, GOAL).


deliberateAid(P1, P2, GOAL, COREGOAL) :-
	attemptToCauseTransitive(P1, GOAL),
	preconditionForTransitive(GOAL, COREGOAL),
	coreGoal(COREGOAL),
	actualizesTransitive(P2, GOAL),
	followedByTransitive(P1, P2).



deliberateAid(P1, P2, GOAL, COREGOAL) :-
	attemptToPreventTransitive(P1, GOAL),
	wouldPreventTransitive(GOAL, COREGOAL),
	coreGoal(COREGOAL),
	ceasesTransitive(P2, GOAL),
	followedByTransitive(P1, P2).


deliberateHarm(P1, P2, GOAL, COREGOAL) :-
	attemptToCauseTransitive(P1, GOAL),
	wouldPreventTransitive(GOAL, COREGOAL),
	coreGoal(COREGOAL),
	actualizesTransitive(P2, GOAL),
	followedByTransitive(P1, P2).


deliberateHarm(P1, P2, GOAL, COREGOAL) :-
	attemptToPreventTransitive(P1, GOAL),
	preconditionForTransitive(GOAL, COREGOAL),
	coreGoal(COREGOAL),
	ceasesTransitive(P2, GOAL),
	followedByTransitive(P1, P2).

	
unintendedAid(P1, INTENDED, UNINTENDED, COREGOAL) :-
	coreGoal(COREGOAL),
	intention(P1, INTENDED),
	actualizesAid(P1, UNINTENDED, COREGOAL),
	INTENDED \== UNINTENDED,
	agent(P1, AGENT),
	\+ declaresIntention(_,INTENDED,AGENT).

	
unintendedHarm(P1, INTENDED, UNINTENDED, COREGOAL) :-
	coreGoal(COREGOAL),
	intention(P1, INTENDED),
	actualizesHarm(P1, UNINTENDED, COREGOAL),
	INTENDED \== UNINTENDED,
	agent(P1, AGENT),
	\+ declaresIntention(_,INTENDED,AGENT).
	
% TODO update diagram
backfireType1(P1, COREGOAL) :-
	coreGoal(COREGOAL),
	attemptToCauseTransitive(P1, COREGOAL),
	actualizesHarm(P1, COREGOAL).


backfireType2(P1, AGENT, PLAN, INTENDED_COREGOAL, UNINTENDED_COREGOAL) :-
	declaresIntention(P1, PLAN, AGENT),
	interpNodeIn(PLAN, GOALBOX_1),
	wouldCauseTransitive(PLAN, INTENDED_COREGOAL),
	interpNodeIn(INTENDED_COREGOAL, GOALBOX_2),
	agent(P1, AGENT),
	agent(GOALBOX_1, AGENT),
	agent(GOALBOX_2, AGENT),
	wouldPreventTransitive(PLAN, UNINTENDED_COREGOAL),
	\+ declaresIntention(_,UNINTENDED_COREGOAL,AGENT),
	coreGoal(INTENDED_COREGOAL),
	coreGoal(UNINTENDED_COREGOAL).


lostOpportunity(P1, P2, P3, PRECONDITION, GOAL, AGENT) :-
	actualizesTransitive(P1, PRECONDITION),
	preconditionForTransitive(PRECONDITION, GOAL),
	ceasesTransitive(P3, PRECONDITION),
	goalDeclared(P2, GOAL, GOALBOX),
	agent(GOALBOX, AGENT),
	followedByTransitive(P1, P2),
	followedByTransitive(P2, P3).


goodSideEffect(P1, AGENT, GOAL, INTENDED_COREGOAL, UNINTENDED, UNINTENDED_COREGOAL) :-
	actualizesAid(P1, GOAL, INTENDED_COREGOAL),
	actualizesAid(P1, UNINTENDED, UNINTENDED_COREGOAL),
	interpNodeIn(GOAL, GOALBOX),
	agent(P1, AGENT),
	agent(GOALBOX, AGENT),
	\+ declaresIntention(_,UNINTENDED_COREGOAL,AGENT).


badSideEffect(P1, AGENT, GOAL, INTENDED_COREGOAL, UNINTENDED, UNINTENDED_COREGOAL) :-
	actualizesAid(P1, GOAL, INTENDED_COREGOAL),
	actualizesHarm(P1, UNINTENDED, UNINTENDED_COREGOAL),
	interpNodeIn(GOAL, GOALBOX),
	agent(P1, AGENT),
	agent(GOALBOX, AGENT),
	\+ declaresIntention(_,UNINTENDED_COREGOAL,AGENT).


% TODO update diagram (same P agent)
recovery(P1, P2, P3, AGENT, DAMAGER, FIXER_GOAL, COREGOAL) :-
	actualizesHarm(P1, DAMAGER, COREGOAL),
	goalDeclared(P2, FIXER_GOAL, _),
	wouldCauseTransitive(FIXER_GOAL, COREGOAL),
	actualizesTransitive(P3, FIXER_GOAL),
	agent(P1, AGENT),
	agent(P2, AGENT),
	followedByOrSimultaneous(P1, P2),
	followedByTransitive(P2, P3).




%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.6: Complex Single-Agent Goal Outcomes %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

peripeteia(P1, P2, P3, BELIEF, EXPECTATION, AGENT) :-
	declaresExpectationToCause(P1, BELIEF, EXPECTATION, AGENT),
	wouldAid(EXPECTATION, COREGOAL, AGENT),
	agent(COREGOAL, AGENT),
	actualizesTransitive(P2, BELIEF),
	ceasesTransitive(P3, EXPECTATION),
	followedByTransitive(P1, P2),
	followedByTransitive(P2, P3),
	actualizesHarm(P3, COREGOAL2),
	agent(COREGOAL2, AGENT).


% TOOD update diagram (coregoal agent)
goalSubstitution(P1, P2, P3, GOAL, NEWGOAL, AGENT) :-
	failure(P1, P2, GOAL, GOALBOX),
	wouldAid(GOAL, COREGOAL, AGENT),
	agent(GOALBOX, AGENT),
	causalTimelinePropositions(P2, P3),
	declaresIntention(P3, NEWGOAL, AGENT),
	wouldAid(NEWGOAL, COREGOAL, AGENT),
	interpNodeIn(NEWGOAL, NEWGOALBOX),
	agent(NEWGOALBOX, AGENT).


failureGivingUp(P1, P2, P3, GOAL, GOALBOX) :-
	failure(P1, P2, GOAL, GOALBOX),
	ceasesTransitive(P3, GOALBOX),
	followedByOrSimultaneous(P2, P3).



noir(P1, P2, P3, GOAL, COREGOAL, COREGOAL2, AGENT) :-
	attemptToCauseTransitive(P1,GOAL),
	wouldAid(GOAL, COREGOAL, AGENT),
	agent(P1, AGENT),
	actualizesHarm(P1, COREGOAL2),
	agent(COREGOAL2, AGENT),
	declaresIntention(P3, COREGOAL2, AGENT),
	followedByOrSimultaneous(P2, P3).


obviatedPlan(P1, P2, AGENT, PLAN, OBJECTIVE) :-
	declaresExpectationToCause(P1, PLAN, OBJECTIVE, AGENT),
	interpNodeIn(PLAN, GOALBOX),
	goalBox(GOALBOX),
	agent(GOALBOX, AGENT),
	followedByTransitive(P1, P2),
	actualizesTransitive(P2, OBJECTIVE),
	\+ actualizesTransitive(_, PLAN).


wastedEffortIrony(P1, P2, P3, P4, P5, AGENT, PLAN, OBJECTIVE) :-
	declaresExpectationToCause(P1, PLAN, OBJECTIVE, AGENT),
	interpNodeIn(PLAN, GOALBOX),
	goalBox(GOALBOX),
	agent(GOALBOX, AGENT),
	followedByOrSimultaneous(P1, P2),

	% OBJECTIVE is actualized because PLAN is.
	actualizesTransitive(P2, PLAN),
	followedByOrSimultaneous(P2, P3),
	actualizesTransitive(P3, OBJECTIVE),

	% OBJECTIVE is later actualized at P5 because of what happened
	% at P4, which is not an actualization of PLAN.
	followedByTransitive(P3, P4),
	causalTimelinePropositions(P4, P5),
	actualizesTransitive(P5, OBJECTIVE),
	\+ actualizesTransitive(P4, PLAN).


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.7: Complex Single-Agent Goal Outcomes %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

mistakenBelief(P1, P2, TRUTH, BELIEF) :-
	actualizesTransitive(P1, TRUTH),
	declaresBelief(P2, BELIEF, _),
	inverseOf(TRUTH, BELIEF).


mistakenBelief(P1, P2, FALSEHOOD, BELIEF) :-
	ceasesTransitive(P1, FALSEHOOD),
	declaresBelief(P2, BELIEF, _),
	equivalentOf(FALSEHOOD, BELIEF).


mistakenBelief(P1, P2, BELIEF) :-
	declaresBelief(P1, BELIEF, _),
	ceasesTransitive(P2, BELIEF).


violatedExpectation(P1, P2, P3, BELIEF, EXPECTATION, AGENT) :-
	declaresExpectationToCause(P1, BELIEF, EXPECTATION, AGENT),
	actualizesTransitive(P2, BELIEF),
	ceasesTransitive(P3, EXPECTATION),
	followedByTransitive(P1, P2),
	followedByTransitive(P2, P3).


surprise(P1, P2, P3, BELIEF, EXPECTATION, AGENT) :-
	declaresExpectationToPrevent(P1, BELIEF, EXPECTATION, AGENT),
	actualizesTransitive(P2, BELIEF),
	actualizesTransitive(P3, BELIEF),
	followedByTransitive(P1, P2),
	followedByTransitive(P2, P3).
  

anagroisis(P1, P2, P3, PRIOR_BELIEF, NEW_BELIEF, AGENT) :-
	declaresBelief(P1, _, AGENT, PRIOR_BELIEF),
	wouldPrevent(NEW_BELIEF, PRIOR_BELIEF),
	declaresBelief(P2, _, AGENT, NEW_BELIEF),
	followedByTransitive(P1, P2),
	ceasesTransitive(P3, PRIOR_BELIEF),
	followedByOrSimultaneous(P2, P3).


anagroisis(P1, P2, P3, PRIOR_BELIEF, NEW_BELIEF, AGENT) :-
	declaresBelief(P1, PRIOR_CONTENT, AGENT, PRIOR_BELIEF),
	wouldPrevent(NEW_CONTENT, PRIOR_CONTENT),
	declaresBelief(P2, NEW_CONTENT, AGENT, NEW_BELIEF),
	followedByTransitive(P1, P2),
	ceasesTransitive(P3, PRIOR_BELIEF),
	followedByOrSimultaneous(P2, P3).


  
anagroisis(P1, P2, _, BELIEF, _, AGENT) :-
	declaresBelief(P1, BELIEF, AGENT, BELIEFBOX),
	ceasesBelief(P2, BELIEF, AGENT, BELIEFBOX),
	followedByTransitive(P1, P2).


potentialContradiction(P1, P2, POSSIBILITY1, POSSIBILITY2, CONFLICT, AGENT) :-
  	declaresExpectationToCause(P1, POSSIBILITY1, CONFLICT, AGENT),
  	declaresExpectationToPrevent(P2, POSSIBILITY2, CONFLICT, AGENT),
	POSSIBILITY1\==POSSIBILITY2.


contradictoryBeliefs(P1, P2, P3, P4, POSSIBILITY1, POSSIBILITY2, BELIEF1, BELIEF2, CONFLICT, AGENT) :-
	equivalentOf(POSSIBILITY1, BELIEF1),
	equivalentOf(POSSIBILITY2, BELIEF2),
	POSSIBILITY1\==POSSIBILITY2,
  	declaresExpectationToCause(P1, POSSIBILITY1, CONFLICT, AGENT),
  	declaresExpectationToPrevent(P2, POSSIBILITY2, CONFLICT, AGENT),
	declaresBelief(P3, BELIEF1, AGENT),
	declaresBelief(P4, BELIEF2, AGENT).



mistakenSatisfaction(P1, P2, P3, GOAL, TRUTH, BELIEVED_TRUTH, AGENT) :-
	declaresIntention(P1, GOAL, AGENT),
	ceasesTransitive(P2, TRUTH),
	declaresBelief(P3, BELIEVED_TRUTH, AGENT, _),
	equivalentOf(BELIEVED_TRUTH, GOAL),
	inverseOf(BELIEVED_TRUTH, TRUTH),
	followedByTransitive(P1, P2),
	followedByTransitive(P1, P3).



%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.8: Dilemmas %
%%%%%%%%%%%%%%%%%%%%%%%%


dilemmaType1(P1, POSSIBILITY, TRIGGER, CONSEQUENCE1, CONSEQUENCE2, AGENT) :-
  	declaresExpectationToCause(P1, POSSIBILITY, TRIGGER, AGENT),
	wouldAid(TRIGGER, CONSEQUENCE1, AGENT),
	wouldHarm(TRIGGER, CONSEQUENCE2, AGENT),
	agent(POSSIBILITY, AGENT).


dilemmaType2(P1, MUTEX1, MUTEX2, CONSEQUENCE1, CONSEQUENCE2, AGENT) :- 
 	declaresExpectationToCause(P1, MUTEX1, TRIGGER1, AGENT),	
  	declaresExpectationToPrevent(P1, MUTEX1, TRIGGER2, AGENT),	
  	declaresExpectationToCause(P1, MUTEX2, TRIGGER2, AGENT),	
  	declaresExpectationToPrevent(P1, MUTEX2, TRIGGER1, AGENT),	
	\+ equivalentOf(MUTEX1, MUTEX2),
	MUTEX1\==MUTEX2,
	wouldAid(TRIGGER1, CONSEQUENCE1, AGENT),
	wouldAid(TRIGGER2, CONSEQUENCE2, AGENT).


% TODO: Update diagram: prioritization about attempt (intention) not success
goalPrioritization(P1, CONSEQUENCE1, CONSEQUENCE2, AGENT) :-
	attemptToCauseTransitive(P1, TRIGGER),
	wouldAid(TRIGGER, CONSEQUENCE1, AGENT),
	wouldHarm(TRIGGER, CONSEQUENCE2, AGENT),
	agent(P1, AGENT).

goalPrioritization(P1, CONSEQUENCE1, CONSEQUENCE2, AGENT) :-
	attemptToPreventTransitive(P1, TRIGGER),
	wouldAid(TRIGGER, CONSEQUENCE1, AGENT),
	wouldHarm(TRIGGER, CONSEQUENCE2, AGENT),
	agent(P1, AGENT).




%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.9: Two-Agent Interactions %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


selfishAct(P1, P2, TRIGGER, CONSEQUENCE1, CONSEQUENCE2, AGENT, EXPERIENCER) :-
	declaresIntention(P1, TRIGGER, AGENT),
	attemptToCauseTransitive(P2, TRIGGER),
	followedByOrSimultaneous(P1, P2),
	wouldAid(TRIGGER, CONSEQUENCE1, AGENT),
	wouldHarm(TRIGGER, CONSEQUENCE2, EXPERIENCER),
	AGENT\==EXPERIENCER,
	CONSEQUENCE1\==CONSEQUENCE2.


selflessAct(P1, P2, TRIGGER, CONSEQUENCE1, CONSEQUENCE2, AGENT, EXPERIENCER) :-
	declaresIntention(P1, TRIGGER, AGENT),
	attemptToCauseTransitive(P2, TRIGGER),
	followedByOrSimultaneous(P1, P2),
	wouldHarm(TRIGGER, CONSEQUENCE1, AGENT),
	wouldAid(TRIGGER, CONSEQUENCE2, EXPERIENCER),
	AGENT\==EXPERIENCER,
	CONSEQUENCE1\==CONSEQUENCE2.



deliberateAssistance(P1, P2, AGENT, TRIGGER, COREGOAL, EXPERIENCER) :-
	declaresIntention(P1, TRIGGER, AGENT),
	attemptToCauseTransitive(P2, TRIGGER),
	followedByOrSimultaneous(P1, P2),
	wouldAid(TRIGGER, COREGOAL, EXPERIENCER),
	AGENT\==EXPERIENCER.


commonlyPursuedGoal(P1, P2, AGENT1, AGENT2, GOAL) :-
	declaresIntention(P1, GOAL, AGENT1),
	declaresIntention(P2, GOAL, AGENT2),
	AGENT1\==AGENT2.


tandemAttempts(P1, P2, P3, GOAL1, GOAL2, AGENT, EXPERIENCER) :-

	setof((GOAL1, GOAL2, P3), (
	attemptToCauseTransitive(P3, GOAL1),
	attemptToCauseTransitive(P3, GOAL2),
	GOAL1\==GOAL2,
	\+ preconditionForTransitive(GOAL1, GOAL2),
	\+ preconditionForTransitive(GOAL2, GOAL1)), Result),
	member((GOAL1, GOAL2, P3), Result),

	declaresIntention(P1, GOAL1, AGENT),	
	declaresIntention(P2, GOAL2, EXPERIENCER),
	followedByOrSimultaneous(P1, P3),
	followedByOrSimultaneous(P2, P3),
	AGENT\==EXPERIENCER.


tandemAttempts(P1, P2, P3, GOAL1, GOAL2, AGENT, EXPERIENCER) :-

	setof((GOAL1, GOAL2, P3), (
	attemptToCauseTransitive(P3, GOAL1),
	attemptToPreventTransitive(P3, GOAL2),
	GOAL1\==GOAL2,
	\+ wouldPreventTransitive(GOAL1, GOAL2),
	\+ wouldPreventTransitive(GOAL2, GOAL1)), Result),
	member((GOAL1, GOAL2, P3), Result),

	declaresIntention(P1, GOAL1, AGENT),	
	declaresIntention(P2, GOAL2, EXPERIENCER),
	followedByOrSimultaneous(P1, P3),
	followedByOrSimultaneous(P2, P3),
	AGENT\==EXPERIENCER.


conflictType1(P1, P2, AGENT1, AGENT2, GOAL) :-
	attemptToCauseTransitive(P1, GOAL),
	attemptToPreventTransitive(P2, GOAL),
	agent(P1, AGENT1),
	agent(P2, AGENT2),
	AGENT1\==AGENT2.


conflictType2(P1, P2, AGENT1, AGENT2, MUTEX1, MUTEX2, CONSEQUENCE1, CONSEQUENCE2) :-
	declaresExpectationToCause(P1, MUTEX1, CONSEQUENCE1, AGENT1),
	wouldPreventTransitive(MUTEX1, CONSEQUENCE2),
	declaresExpectationToCause(P2, MUTEX2, CONSEQUENCE2, AGENT2),
	wouldPreventTransitive(MUTEX2, CONSEQUENCE1),
	CONSEQUENCE1\==CONSEQUENCE2,
	AGENT1\==AGENT2,
	MUTEX1\==MUTEX2,
	goalOfAgent(CONSEQUENCE1, AGENT1),
	goalOfAgent(CONSEQUENCE2, AGENT2).



giftOfTheMagiIrony(P1, P2, P3, P4, AGENT1, AGENT2, MUTEX1, MUTEX2, CONSEQUENCE1, CONSEQUENCE2) :-
	declaresExpectationToCause(P1, MUTEX1, CONSEQUENCE1, AGENT1),
	declaresExpectationToCause(P2, MUTEX2, CONSEQUENCE2, AGENT2),
	goalOfAgent(CONSEQUENCE1, AGENT1),
	goalOfAgent(CONSEQUENCE2, AGENT2),

	AGENT1\==AGENT2,
	MUTEX1\==MUTEX2,
	CONSEQUENCE1\==CONSEQUENCE2,

	wouldPreventTransitive(MUTEX1, CONSEQUENCE2),
	wouldPreventTransitive(MUTEX2, CONSEQUENCE1),
	wouldAid(CONSEQUENCE1, COREGOAL2, AGENT2),
	wouldAid(CONSEQUENCE2, COREGOAL1, AGENT1),
	wouldHarm(MUTEX1, COREGOAL1, AGENT1),
	wouldHarm(MUTEX2, COREGOAL2, AGENT2),

	followedByOrSimultaneous(P3, P1),
	followedByOrSimultaneous(P3, P2),
	followedByOrSimultaneous(P4, P1),
	followedByOrSimultaneous(P4, P2),

	agent(P3, AGENT1),
	agent(P4, AGENT2),
	attemptToCauseTransitive(P3, MUTEX1),
	attemptToCauseTransitive(P4, MUTEX2),
	ceasesTransitive(P3, CONSEQUENCE2),
	ceasesTransitive(P4, CONSEQUENCE1).



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.10: Persuasion and Deception %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


persuasion(P1, P2, AGENT, EXPERIENCER, THEME) :-	
	success(P1, P2, THEME, GOALBOX),
	agent(GOALBOX, AGENT),
	goalOrBeliefBox(THEME),
	agent(THEME, EXPERIENCER).


deception(P1, P2, P3, P4, AGENT, EXPERIENCER, THEME) :-	
	actualizesTransitive(P1, THEME),
	declaresBelief(P2, THEME2, AGENT),
	inverseOf(THEME_INVERSE, THEME),
	inverseOf(THEME_INVERSE, THEME2),
	followedByOrSimultaneous(P3, P1),
	followedByOrSimultaneous(P3, P2),
	followedByOrSimultaneous(P4, P1),
	followedByOrSimultaneous(P4, P2),
	persuasion(P2, P3, AGENT, EXPERIENCER, BELIEFBOX),
	beliefBox(BELIEFBOX),
	interpNodeIn(THEME_INVERSE, BELIEFBOX).

	


unintendedPersuasion(P1, P2, P3, P4, AGENT, EXPERIENCER, THEME) :-	
	actualizesTransitive(P1, THEME),
	declaresBelief(P2, THEME_INVERSE, AGENT),
	inverseOf(THEME_INVERSE, THEME),
	followedByOrSimultaneous(P3, P1),
	followedByOrSimultaneous(P3, P2),
	followedByOrSimultaneous(P4, P1),
	followedByOrSimultaneous(P4, P2),
	persuasion(P2, P3, AGENT, EXPERIENCER, BELIEFBOX),
	beliefBox(BELIEFBOX),
	interpNodeIn(THEME2, BELIEFBOX),
	equivalentOf(THEME2, THEME).


	
mutualDeception(P1, P2, P3, P4, AGENT, EXPERIENCER, THEME) :-

	actualizesTransitive(P1, THEME),
	declaresBelief(P2, THEME2, AGENT),
	declaresBelief(P3, THEME3, EXPERIENCER),
	AGENT\==EXPERIENCER,

	equivalentOf(THEME2, THEME),
	equivalentOf(THEME3, THEME),

	attemptToCauseTransitive(P4, DECEPTION_GOAL),
	agent(P4, AGENT),
	beliefBox(DECEPTION_GOAL),
	agent(DECEPTION_GOAL, EXPERIENCER),
	interpNodeIn(INVERSE_THEME, DECEPTION_GOAL),
	inverseOf(INVERSE_THEME, THEME),

	attemptToCauseTransitive(P5, COUNTER_DECEPTION_GOAL),
	agent(P5, EXPERIENCER),
	beliefBox(COUNTER_DECEPTION_GOAL),
	agent(COUNTER_DECEPTION_GOAL, AGENT),

	interpNodeIn(COUNTER_DECEPTION_GOAL_INNER, COUNTER_DECEPTION_GOAL),
	beliefBox(COUNTER_DECEPTION_GOAL_INNER),
	agent(COUNTER_DECEPTION_GOAL_INNER, EXPERIENCER),
	interpNodeIn(INVERSE_THEME_2, COUNTER_DECEPTION_GOAL_INNER),
	inverseOf(INVERSE_THEME_2, THEME),

	followedBy(P4, P1),
	followedBy(P4, P2),
	followedBy(P4, P3),
	followedBy(P5, P4).

	

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.11: Persuasion and Deception %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

motivatedToRevenge(P1, P2, P3, AGENT, EXPERIENCER) :-
	deliberateHarm(P1, P2, _, COREGOAL),
	agent(P1, AGENT),
	agent(COREGOAL, EXPERIENCER),
	AGENT\==EXPERIENCER,

	followedByTransitive(P2, P3),
	causalTimelinePropositions(P2, P3),
	declaresIntention(P3, REVENGE_GOAL, EXPERIENCER),
	wouldHarm(REVENGE_GOAL, _, AGENT).


motivatedToReturnFavor(P1, P2, P3, AGENT, EXPERIENCER) :-
	deliberateAid(P1, P2, _, COREGOAL),
	agent(P1, AGENT),
	agent(COREGOAL, EXPERIENCER),
	AGENT\==EXPERIENCER,

	followedByTransitive(P2, P3),
	causalTimelinePropositions(P2, P3),
	declaresIntention(P3, REVENGE_GOAL, EXPERIENCER),
	wouldAid(REVENGE_GOAL, _, AGENT).


% TODO: redraw these, possibly, to remove forced equivalence of 
% same content... this is talked about in the text but is it really necessary?
successfulCoercion(P1, P2, P3, AGENT, EXPERIENCER) :-
	declaresExpectationToCause(P1, THREAT, ULTIMATE_GOAL, AGENT),
	goalBox(THREAT),
	agent(THREAT, EXPERIENCER),
	interpNodeIn(COERCED_ACTION, THREAT),
	wouldAid(COERCED_ACTION, _, EXPERIENCER),
	wouldAid(ULTIMATE_GOAL, _, AGENT),
	actualizesTransitive(P2, THREAT),
	actualizesTransitive(P3, ULTIMATE_GOAL),
	followedByOrSimultaneous(P2, P1),
	followedByOrSimultaneous(P3, P2).



hiddenAgenda(P1, AGENT, PURPORTED, ULTIMATE_GOAL, EXPERIENCER) :-
	declaresExpectationToCause(P1, PURPORTED, ULTIMATE_GOAL, AGENT),
	goalBox(PURPORTED),
	agent(PURPORTED, EXPERIENCER),
	interpNodeIn(PROPOSED_ACTION, PURPORTED),
	wouldAid(PROPOSED_ACTION, _, EXPERIENCER),
	wouldAid(ULTIMATE_GOAL, _, AGENT).



betrayal(P1, P2, P3, P4, AGENT, EXPERIENCER, ACTION) :-
	success(P1, P3, BELIEF, _),
 	beliefBox(BELIEF),
	agent(BELIEF, EXPERIENCER),

	interpNodeIn(PURPORTED_GOAL, BELIEF),
	goalBox(PURPORTED_GOAL),
	agent(PURPORTED_GOAL, AGENT),
	ceasesTransitive(P2, PURPORTED_GOAL),
	followedByTransitive(P2, P3),

	interpNodeIn(ACTION, PURPORTED_GOAL),
	wouldAid(ACTION, _, EXPERIENCER),

	followedByTransitive(P3, P4),
	ceasesTransitive(P4, ACTION).


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.12: Manipulation of Time %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


flashback(P1, P2) :-
	sourceTextBeginOffset(P1, S1),

	followedBy(P1, P2),
	sourceTextBeginOffset(P2, S2),
	S2 @< S1,

	followedBy(P2, P3),
	sourceTextBeginOffset(P3, S3),
	S3 @< S1,
	S3 @> S2,

	followedByTransitive(P3, P4),
	sourceTextBeginOffset(P4, S4),
	S4 @> S1.



flashforward(P1, P2) :-
	sourceTextBeginOffset(P1, S1),

	followedBy(P1, P2),
	sourceTextBeginOffset(P2, S2),
	S2 @> S1,

	followedBy(P2, P3),
	sourceTextBeginOffset(P3, S3),
	S3 @> S2,

	followedByTransitive(P3, P4),
	sourceTextBeginOffset(P4, S4),
	S4 @> S1,
	S4 @< S2.


% Update diagram: no need for top row
suspense(P1, P2, P3, PROMISE, POTENTIAL) :-
	followedBy(P1, P2),
	followedBy(P2, P3),
	promiseOrThreat(P1, POTENTIAL, PROMISE, _),
	\+ actualizesTransitive(P1, POTENTIAL),
	\+ ceasesTransitive(P1, POTENTIAL),
	\+ actualizesTransitive(P2, POTENTIAL),
	\+ ceasesTransitive(P2, POTENTIAL),
	\+ actualizesTransitive(P3, POTENTIAL),
	\+ ceasesTransitive(P3, POTENTIAL).



%%%%%%%%%%%%%%%%%%%%%%%%
% FIGURE B.13: Mystery %
%%%%%%%%%%%%%%%%%%%%%%%%

mysteryType1(P1, P2, MYSTERIOUS_EVENT) :-
	causalTimelinePropositions(P1, P2, _, MYSTERIOUS_EVENT),
	sourceTextBeginOffset(P1, S1),
	sourceTextBeginOffset(P2, S2),
	S1 @> S2.


mysteryType2(P1, P2, MYSTERIOUS_EVENT) :-
	causalTimelinePropositions(P1, P2, _, MYSTERIOUS_EVENT),
	interpNodeIn(P1, BELIEFBOX), % Alternate timeline
	followedByTransitive(P2, P3),
	actualizesTransitive(P3, BELIEFBOX). % Revelation


mysteryType3(P1, P2, MYSTERIOUS_EVENT, AGENT) :-
	attemptToCauseTransitive(P1, MYSTERIOUS_EVENT),
	interpNodeIn(MYSTERIOUS_EVENT, GOALBOX),
	agent(GOALBOX, AGENT),
	agent(P1, AGENT),
	\+ declaredPrior(GOALBOX, P1),
	followedByTransitive(P1, P2),
	actualizesTransitive(P2, MYSTERIOUS_EVENT).


declaredPrior(GOALBOX, CUTOFF_TIME) :-
	followedByTransitive(P1, CUTOFF_TIME),
	actualizesFlat(P1, GOALBOX).


mystery(P1, P2, MYSTERIOUS_EVENT) :-
	mysteryType1(P1, P2, MYSTERIOUS_EVENT).
mystery(P1, P2, MYSTERIOUS_EVENT) :-
	mysteryType2(P1, P2, MYSTERIOUS_EVENT).
mystery(P1, P2, MYSTERIOUS_EVENT) :-
	mysteryType3(P1, P2, MYSTERIOUS_EVENT, _).




run_gain :-
	encoding(ENC),
	setof((A,B), gain(A,B),Answergain),length(Answergain,L1),write(ENC),write('\tSTATIC FUNCTION	gain\t'),write(L1),write('\n').
run_gain :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	gain\t0\n').
run_loss :-
	encoding(ENC),
	setof((A,B), loss(A,B),Answerloss),length(Answerloss,L2),write(ENC),write('\tSTATIC FUNCTION	loss\t'),write(L2),write('\n').
run_loss :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	loss\t0\n').
run_negativeResolution :-
	encoding(ENC),
	setof((A,B,C), negativeResolution(A,B,C),AnswernegativeResolution),length(AnswernegativeResolution,L3),write(ENC),write('\tSTATIC FUNCTION	negativeResolution\t'),write(L3),write('\n').
run_negativeResolution :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	negativeResolution\t0\n').
run_positiveResolution :-
	encoding(ENC),
	setof((A,B,C), positiveResolution(A,B,C),AnswerpositiveResolution),length(AnswerpositiveResolution,L4),write(ENC),write('\tSTATIC FUNCTION	positiveResolution\t'),write(L4),write('\n').
run_positiveResolution :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	positiveResolution\t0\n').
run_complexPositive :-
	encoding(ENC),
	setof((A,B,C,D,E), complexPositive(A,B,C,D,E),AnswercomplexPositive),length(AnswercomplexPositive,L5),write(ENC),write('\tSTATIC FUNCTION	complexPositive\t'),write(L5),write('\n').
run_complexPositive :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	complexPositive\t0\n').
run_complexNegative :-
	encoding(ENC),
	setof((A,B,C,D,E), complexNegative(A,B,C,D,E),AnswercomplexNegative),length(AnswercomplexNegative,L6),write(ENC),write('\tSTATIC FUNCTION	complexNegative\t'),write(L6),write('\n').
run_complexNegative :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	complexNegative\t0\n').
run_hiddenBlessing :-
	encoding(ENC),
	setof((A,B,C,D), hiddenBlessing(A,B,C,D),AnswerhiddenBlessing),length(AnswerhiddenBlessing,L7),write(ENC),write('\tSTATIC FUNCTION	hiddenBlessing\t'),write(L7),write('\n').
run_hiddenBlessing :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	hiddenBlessing\t0\n').
run_positiveTradeoff :-
	encoding(ENC),
	setof((A,B,C,D), positiveTradeoff(A,B,C,D),AnswerpositiveTradeoff),length(AnswerpositiveTradeoff,L8),write(ENC),write('\tSTATIC FUNCTION	positiveTradeoff\t'),write(L8),write('\n').
run_positiveTradeoff :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	positiveTradeoff\t0\n').
run_negativeTradeoff :-
	encoding(ENC),
	setof((A,B,C,D), negativeTradeoff(A,B,C,D),AnswernegativeTradeoff),length(AnswernegativeTradeoff,L9),write(ENC),write('\tSTATIC FUNCTION	negativeTradeoff\t'),write(L9),write('\n').
run_negativeTradeoff :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	negativeTradeoff\t0\n').
run_mixedBlessing :-
	encoding(ENC),
	setof((A,B,C,D), mixedBlessing(A,B,C,D),AnswermixedBlessing),length(AnswermixedBlessing,L10),write(ENC),write('\tSTATIC FUNCTION	mixedBlessing\t'),write(L10),write('\n').
run_mixedBlessing :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	mixedBlessing\t0\n').
run_promise :-
	encoding(ENC),
	setof((A,B,C,D), promise(A,B,C,D),Answerpromise),length(Answerpromise,L11),write(ENC),write('\tSTATIC FUNCTION	promise\t'),write(L11),write('\n').
run_promise :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	promise\t0\n').
run_threat :-
	encoding(ENC),
	setof((A,B,C,D), threat(A,B,C,D),Answerthreat),length(Answerthreat,L12),write(ENC),write('\tSTATIC FUNCTION	threat\t'),write(L12),write('\n').
run_threat :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	threat\t0\n').
run_promiseFulfilled :-
	encoding(ENC),
	setof((A,B,C,D,E), promiseFulfilled(A,B,C,D,E),AnswerpromiseFulfilled),length(AnswerpromiseFulfilled,L13),write(ENC),write('\tSTATIC FUNCTION	promiseFulfilled\t'),write(L13),write('\n').
run_promiseFulfilled :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	promiseFulfilled\t0\n').
run_threatFulfilled :-
	encoding(ENC),
	setof((A,B,C,D,E), threatFulfilled(A,B,C,D,E),AnswerthreatFulfilled),length(AnswerthreatFulfilled,L14),write(ENC),write('\tSTATIC FUNCTION	threatFulfilled\t'),write(L14),write('\n').
run_threatFulfilled :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	threatFulfilled\t0\n').
run_promiseBroken :-
	encoding(ENC),
	setof((A,B,C,D,E), promiseBroken(A,B,C,D,E),AnswerpromiseBroken),length(AnswerpromiseBroken,L15),write(ENC),write('\tSTATIC FUNCTION	promiseBroken\t'),write(L15),write('\n').
run_promiseBroken :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	promiseBroken\t0\n').
run_threatAvoided :-
	encoding(ENC),
	setof((A,B,C,D,E), threatAvoided(A,B,C,D,E),AnswerthreatAvoided),length(AnswerthreatAvoided,L16),write(ENC),write('\tSTATIC FUNCTION	threatAvoided\t'),write(L16),write('\n').
run_threatAvoided :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	threatAvoided\t0\n').
run_partialResolution :-
	encoding(ENC),
	setof((A,B,C,D), partialResolution(A,B,C,D),AnswerpartialResolution),length(AnswerpartialResolution,L17),write(ENC),write('\tSTATIC FUNCTION	partialResolution\t'),write(L17),write('\n').
run_partialResolution :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	partialResolution\t0\n').
run_compoundedTransition :-
	encoding(ENC),
	setof((A,B,C,D,E), compoundedTransition(A,B,C,D,E),AnswercompoundedTransition),length(AnswercompoundedTransition,L18),write(ENC),write('\tSTATIC FUNCTION	compoundedTransition\t'),write(L18),write('\n').
run_compoundedTransition :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	compoundedTransition\t0\n').
run_goalDeclared :-
	encoding(ENC),
	setof((A,B,C), goalDeclared(A,B,C),AnswergoalDeclared),length(AnswergoalDeclared,L19),write(ENC),write('\tSTATIC FUNCTION	goalDeclared\t'),write(L19),write('\n').
run_goalDeclared :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalDeclared\t0\n').
run_desireToAid :-
	encoding(ENC),
	setof((A,B,C,D), desireToAid(A,B,C,D),AnswerdesireToAid),length(AnswerdesireToAid,L20),write(ENC),write('\tSTATIC FUNCTION	desireToAid\t'),write(L20),write('\n').
run_desireToAid :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	desireToAid\t0\n').
run_desireToHarm :-
	encoding(ENC),
	setof((A,B,C,D), desireToHarm(A,B,C,D),AnswerdesireToHarm),length(AnswerdesireToHarm,L21),write(ENC),write('\tSTATIC FUNCTION	desireToHarm\t'),write(L21),write('\n').
run_desireToHarm :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	desireToHarm\t0\n').
run_explicitMotivation :-
	encoding(ENC),
	setof((A,B,C), explicitMotivation(A,B,C),AnswerexplicitMotivation),length(AnswerexplicitMotivation,L22),write(ENC),write('\tSTATIC FUNCTION	explicitMotivation\t'),write(L22),write('\n').
run_explicitMotivation :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	explicitMotivation\t0\n').
run_problem :-
	encoding(ENC),
	setof((A,B,C,D,E,F), problem(A,B,C,D,E,F),Answerproblem),length(Answerproblem,L23),write(ENC),write('\tSTATIC FUNCTION	problem\t'),write(L23),write('\n').
run_problem :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	problem\t0\n').
run_changeOfMind :-
	encoding(ENC),
	setof((A,B,C,D), changeOfMind(A,B,C,D),AnswerchangeOfMind),length(AnswerchangeOfMind,L24),write(ENC),write('\tSTATIC FUNCTION	changeOfMind\t'),write(L24),write('\n').
run_changeOfMind :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	changeOfMind\t0\n').
run_goalEnablement :-
	encoding(ENC),
	setof((A,B,C,D,E), goalEnablement(A,B,C,D,E),AnswergoalEnablement),length(AnswergoalEnablement,L25),write(ENC),write('\tSTATIC FUNCTION	goalEnablement\t'),write(L25),write('\n').
run_goalEnablement :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalEnablement\t0\n').
run_goalObstacle :-
	encoding(ENC),
	setof((A,B,C,D,E), goalObstacle(A,B,C,D,E),AnswergoalObstacle),length(AnswergoalObstacle,L26),write(ENC),write('\tSTATIC FUNCTION	goalObstacle\t'),write(L26),write('\n').
run_goalObstacle :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalObstacle\t0\n').
run_goalSuccessExpected :-
	encoding(ENC),
	setof((A,B,C,D,E), goalSuccessExpected(A,B,C,D,E),AnswergoalSuccessExpected),length(AnswergoalSuccessExpected,L27),write(ENC),write('\tSTATIC FUNCTION	goalSuccessExpected\t'),write(L27),write('\n').
run_goalSuccessExpected :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalSuccessExpected\t0\n').
run_goalFailureExpected :-
	encoding(ENC),
	setof((A,B,C,D,E), goalFailureExpected(A,B,C,D,E),AnswergoalFailureExpected),length(AnswergoalFailureExpected,L28),write(ENC),write('\tSTATIC FUNCTION	goalFailureExpected\t'),write(L28),write('\n').
run_goalFailureExpected :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalFailureExpected\t0\n').
run_goalAvoidance :-
	encoding(ENC),
	setof((A,B,C,D,E), goalAvoidance(A,B,C,D,E),AnswergoalAvoidance),length(AnswergoalAvoidance,L29),write(ENC),write('\tSTATIC FUNCTION	goalAvoidance\t'),write(L29),write('\n').
run_goalAvoidance :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalAvoidance\t0\n').
run_goalPreemption :-
	encoding(ENC),
	setof((A,B,C,D,E,F), goalPreemption(A,B,C,D,E,F),AnswergoalPreemption),length(AnswergoalPreemption,L30),write(ENC),write('\tSTATIC FUNCTION	goalPreemption\t'),write(L30),write('\n').
run_goalPreemption :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalPreemption\t0\n').
run_perseverance :-
	encoding(ENC),
	setof((A,B,C,D), perseverance(A,B,C,D),Answerperseverance),length(Answerperseverance,L31),write(ENC),write('\tSTATIC FUNCTION	perseverance\t'),write(L31),write('\n').
run_perseverance :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	perseverance\t0\n').
run_success :-
	encoding(ENC),
	setof((A,B,C,D), success(A,B,C,D),Answersuccess),length(Answersuccess,L32),write(ENC),write('\tSTATIC FUNCTION	success\t'),write(L32),write('\n').
run_success :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	success\t0\n').
run_failure :-
	encoding(ENC),
	setof((A,B,C,D), failure(A,B,C,D),Answerfailure),length(Answerfailure,L33),write(ENC),write('\tSTATIC FUNCTION	failure\t'),write(L33),write('\n').
run_failure :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	failure\t0\n').
run_deliberateAid :-
	encoding(ENC),
	setof((A,B,C,D), deliberateAid(A,B,C,D),AnswerdeliberateAid),length(AnswerdeliberateAid,L34),write(ENC),write('\tSTATIC FUNCTION	deliberateAid\t'),write(L34),write('\n').
run_deliberateAid :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	deliberateAid\t0\n').
run_deliberateHarm :-
	encoding(ENC),
	setof((A,B,C,D), deliberateHarm(A,B,C,D),AnswerdeliberateHarm),length(AnswerdeliberateHarm,L35),write(ENC),write('\tSTATIC FUNCTION	deliberateHarm\t'),write(L35),write('\n').
run_deliberateHarm :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	deliberateHarm\t0\n').
run_unintendedAid :-
	encoding(ENC),
	setof((A,B,C,D), unintendedAid(A,B,C,D),AnswerunintendedAid),length(AnswerunintendedAid,L36),write(ENC),write('\tSTATIC FUNCTION	unintendedAid\t'),write(L36),write('\n').
run_unintendedAid :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	unintendedAid\t0\n').
run_unintendedHarm :-
	encoding(ENC),
	setof((A,B,C,D), unintendedHarm(A,B,C,D),AnswerunintendedHarm),length(AnswerunintendedHarm,L37),write(ENC),write('\tSTATIC FUNCTION	unintendedHarm\t'),write(L37),write('\n').
run_unintendedHarm :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	unintendedHarm\t0\n').
run_backfireType1 :-
	encoding(ENC),
	setof((A,B), backfireType1(A,B),AnswerbackfireType1),length(AnswerbackfireType1,L38),write(ENC),write('\tSTATIC FUNCTION	backfireType1\t'),write(L38),write('\n').
run_backfireType1 :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	backfireType1\t0\n').
run_backfireType2 :-
	encoding(ENC),
	setof((A,B,C,D,E), backfireType2(A,B,C,D,E),AnswerbackfireType2),length(AnswerbackfireType2,L39),write(ENC),write('\tSTATIC FUNCTION	backfireType2\t'),write(L39),write('\n').
run_backfireType2 :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	backfireType2\t0\n').
run_lostOpportunity :-
	encoding(ENC),
	setof((A,B,C,D,E,F), lostOpportunity(A,B,C,D,E,F),AnswerlostOpportunity),length(AnswerlostOpportunity,L40),write(ENC),write('\tSTATIC FUNCTION	lostOpportunity\t'),write(L40),write('\n').
run_lostOpportunity :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	lostOpportunity\t0\n').
run_goodSideEffect :-
	encoding(ENC),
	setof((A,B,C,D,E,F), goodSideEffect(A,B,C,D,E,F),AnswergoodSideEffect),length(AnswergoodSideEffect,L41),write(ENC),write('\tSTATIC FUNCTION	goodSideEffect\t'),write(L41),write('\n').
run_goodSideEffect :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goodSideEffect\t0\n').
run_badSideEffect :-
	encoding(ENC),
	setof((A,B,C,D,E,F), badSideEffect(A,B,C,D,E,F),AnswerbadSideEffect),length(AnswerbadSideEffect,L42),write(ENC),write('\tSTATIC FUNCTION	badSideEffect\t'),write(L42),write('\n').
run_badSideEffect :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	badSideEffect\t0\n').
run_recovery :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), recovery(A,B,C,D,E,F,G),Answerrecovery),length(Answerrecovery,L43),write(ENC),write('\tSTATIC FUNCTION	recovery\t'),write(L43),write('\n').
run_recovery :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	recovery\t0\n').
run_peripeteia :-
	encoding(ENC),
	setof((A,B,C,D,E,F), peripeteia(A,B,C,D,E,F),Answerperipeteia),length(Answerperipeteia,L44),write(ENC),write('\tSTATIC FUNCTION	peripeteia\t'),write(L44),write('\n').
run_peripeteia :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	peripeteia\t0\n').
run_goalSubstitution :-
	encoding(ENC),
	setof((A,B,C,D,E,F), goalSubstitution(A,B,C,D,E,F),AnswergoalSubstitution),length(AnswergoalSubstitution,L45),write(ENC),write('\tSTATIC FUNCTION	goalSubstitution\t'),write(L45),write('\n').
run_goalSubstitution :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalSubstitution\t0\n').
run_failureGivingUp :-
	encoding(ENC),
	setof((A,B,C,D,E), failureGivingUp(A,B,C,D,E),AnswerfailureGivingUp),length(AnswerfailureGivingUp,L46),write(ENC),write('\tSTATIC FUNCTION	failureGivingUp\t'),write(L46),write('\n').
run_failureGivingUp :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	failureGivingUp\t0\n').
run_noir :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), noir(A,B,C,D,E,F,G),Answernoir),length(Answernoir,L47),write(ENC),write('\tSTATIC FUNCTION	noir\t'),write(L47),write('\n').
run_noir :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	noir\t0\n').
run_obviatedPlan :-
	encoding(ENC),
	setof((A,B,C,D,E), obviatedPlan(A,B,C,D,E),AnswerobviatedPlan),length(AnswerobviatedPlan,L48),write(ENC),write('\tSTATIC FUNCTION	obviatedPlan\t'),write(L48),write('\n').
run_obviatedPlan :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	obviatedPlan\t0\n').
run_wastedEffortIrony :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G,H), wastedEffortIrony(A,B,C,D,E,F,G,H),AnswerwastedEffortIrony),length(AnswerwastedEffortIrony,L49),write(ENC),write('\tSTATIC FUNCTION	wastedEffortIrony\t'),write(L49),write('\n').
run_wastedEffortIrony :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	wastedEffortIrony\t0\n').
run_mistakenBelief :-
	encoding(ENC),
	setof((A,B,C,D), mistakenBelief(A,B,C,D),AnswermistakenBelief),length(AnswermistakenBelief,L50),write(ENC),write('\tSTATIC FUNCTION	mistakenBelief\t'),write(L50),write('\n').
run_mistakenBelief :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	mistakenBelief\t0\n').
run_violatedExpectation :-
	encoding(ENC),
	setof((A,B,C,D,E,F), violatedExpectation(A,B,C,D,E,F),AnswerviolatedExpectation),length(AnswerviolatedExpectation,L51),write(ENC),write('\tSTATIC FUNCTION	violatedExpectation\t'),write(L51),write('\n').
run_violatedExpectation :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	violatedExpectation\t0\n').
run_surprise :-
	encoding(ENC),
	setof((A,B,C,D,E,F), surprise(A,B,C,D,E,F),Answersurprise),length(Answersurprise,L52),write(ENC),write('\tSTATIC FUNCTION	surprise\t'),write(L52),write('\n').
run_surprise :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	surprise\t0\n').
run_anagroisis :-
	encoding(ENC),
	setof((A,B,C,D,E,F), anagroisis(A,B,C,D,E,F),Answeranagroisis),length(Answeranagroisis,L53),write(ENC),write('\tSTATIC FUNCTION	anagroisis\t'),write(L53),write('\n').
run_anagroisis :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	anagroisis\t0\n').
run_potentialContradiction :-
	encoding(ENC),
	setof((A,B,C,D,E,F), potentialContradiction(A,B,C,D,E,F),AnswerpotentialContradiction),length(AnswerpotentialContradiction,L54),write(ENC),write('\tSTATIC FUNCTION	potentialContradiction\t'),write(L54),write('\n').
run_potentialContradiction :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	potentialContradiction\t0\n').
run_contradictoryBeliefs :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G,H,I,J), contradictoryBeliefs(A,B,C,D,E,F,G,H,I,J),AnswercontradictoryBeliefs),length(AnswercontradictoryBeliefs,L55),write(ENC),write('\tSTATIC FUNCTION	contradictoryBeliefs\t'),write(L55),write('\n').
run_contradictoryBeliefs :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	contradictoryBeliefs\t0\n').
run_mistakenSatisfaction :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), mistakenSatisfaction(A,B,C,D,E,F,G),AnswermistakenSatisfaction),length(AnswermistakenSatisfaction,L56),write(ENC),write('\tSTATIC FUNCTION	mistakenSatisfaction\t'),write(L56),write('\n').
run_mistakenSatisfaction :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	mistakenSatisfaction\t0\n').
run_dilemmaType1 :-
	encoding(ENC),
	setof((A,B,C,D,E,F), dilemmaType1(A,B,C,D,E,F),AnswerdilemmaType1),length(AnswerdilemmaType1,L57),write(ENC),write('\tSTATIC FUNCTION	dilemmaType1\t'),write(L57),write('\n').
run_dilemmaType1 :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	dilemmaType1\t0\n').
run_dilemmaType2 :-
	encoding(ENC),
	setof((A,B,C,D,E,F), dilemmaType2(A,B,C,D,E,F),AnswerdilemmaType2),length(AnswerdilemmaType2,L58),write(ENC),write('\tSTATIC FUNCTION	dilemmaType2\t'),write(L58),write('\n').
run_dilemmaType2 :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	dilemmaType2\t0\n').
run_goalPrioritization :-
	encoding(ENC),
	setof((A,B,C,D), goalPrioritization(A,B,C,D),AnswergoalPrioritization),length(AnswergoalPrioritization,L59),write(ENC),write('\tSTATIC FUNCTION	goalPrioritization\t'),write(L59),write('\n').
run_goalPrioritization :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	goalPrioritization\t0\n').
run_selfishAct :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), selfishAct(A,B,C,D,E,F,G),AnswerselfishAct),length(AnswerselfishAct,L60),write(ENC),write('\tSTATIC FUNCTION	selfishAct\t'),write(L60),write('\n').
run_selfishAct :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	selfishAct\t0\n').
run_selflessAct :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), selflessAct(A,B,C,D,E,F,G),AnswerselflessAct),length(AnswerselflessAct,L61),write(ENC),write('\tSTATIC FUNCTION	selflessAct\t'),write(L61),write('\n').
run_selflessAct :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	selflessAct\t0\n').
run_deliberateAssistance :-
	encoding(ENC),
	setof((A,B,C,D,E,F), deliberateAssistance(A,B,C,D,E,F),AnswerdeliberateAssistance),length(AnswerdeliberateAssistance,L62),write(ENC),write('\tSTATIC FUNCTION	deliberateAssistance\t'),write(L62),write('\n').
run_deliberateAssistance :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	deliberateAssistance\t0\n').
run_commonlyPursuedGoal :-
	encoding(ENC),
	setof((A,B,C,D,E), commonlyPursuedGoal(A,B,C,D,E),AnswercommonlyPursuedGoal),length(AnswercommonlyPursuedGoal,L63),write(ENC),write('\tSTATIC FUNCTION	commonlyPursuedGoal\t'),write(L63),write('\n').
run_commonlyPursuedGoal :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	commonlyPursuedGoal\t0\n').
run_tandemAttempts :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), tandemAttempts(A,B,C,D,E,F,G),AnswertandemAttempts),length(AnswertandemAttempts,L64),write(ENC),write('\tSTATIC FUNCTION	tandemAttempts\t'),write(L64),write('\n').
run_tandemAttempts :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	tandemAttempts\t0\n').
run_conflictType1 :-
	encoding(ENC),
	setof((A,B,C,D,E), conflictType1(A,B,C,D,E),AnswerconflictType1),length(AnswerconflictType1,L65),write(ENC),write('\tSTATIC FUNCTION	conflictType1\t'),write(L65),write('\n').
run_conflictType1 :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	conflictType1\t0\n').
run_conflictType2 :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G,H), conflictType2(A,B,C,D,E,F,G,H),AnswerconflictType2),length(AnswerconflictType2,L66),write(ENC),write('\tSTATIC FUNCTION	conflictType2\t'),write(L66),write('\n').
run_conflictType2 :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	conflictType2\t0\n').
run_giftOfTheMagiIrony :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G,H,I,J), giftOfTheMagiIrony(A,B,C,D,E,F,G,H,I,J),AnswergiftOfTheMagiIrony),length(AnswergiftOfTheMagiIrony,L67),write(ENC),write('\tSTATIC FUNCTION	giftOfTheMagiIrony\t'),write(L67),write('\n').
run_giftOfTheMagiIrony :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	giftOfTheMagiIrony\t0\n').
run_persuasion :-
	encoding(ENC),
	setof((A,B,C,D,E), persuasion(A,B,C,D,E),Answerpersuasion),length(Answerpersuasion,L68),write(ENC),write('\tSTATIC FUNCTION	persuasion\t'),write(L68),write('\n').
run_persuasion :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	persuasion\t0\n').
run_deception :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), deception(A,B,C,D,E,F,G),Answerdeception),length(Answerdeception,L69),write(ENC),write('\tSTATIC FUNCTION	deception\t'),write(L69),write('\n').
run_deception :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	deception\t0\n').
run_unintendedPersuasion :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), unintendedPersuasion(A,B,C,D,E,F,G),AnswerunintendedPersuasion),length(AnswerunintendedPersuasion,L70),write(ENC),write('\tSTATIC FUNCTION	unintendedPersuasion\t'),write(L70),write('\n').
run_unintendedPersuasion :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	unintendedPersuasion\t0\n').
run_mutualDeception :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), mutualDeception(A,B,C,D,E,F,G),AnswermutualDeception),length(AnswermutualDeception,L71),write(ENC),write('\tSTATIC FUNCTION	mutualDeception\t'),write(L71),write('\n').
run_mutualDeception :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	mutualDeception\t0\n').
run_motivatedToRevenge :-
	encoding(ENC),
	setof((A,B,C,D,E), motivatedToRevenge(A,B,C,D,E),AnswermotivatedToRevenge),length(AnswermotivatedToRevenge,L72),write(ENC),write('\tSTATIC FUNCTION	motivatedToRevenge\t'),write(L72),write('\n').
run_motivatedToRevenge :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	motivatedToRevenge\t0\n').
run_motivatedToReturnFavor :-
	encoding(ENC),
	setof((A,B,C,D,E), motivatedToReturnFavor(A,B,C,D,E),AnswermotivatedToReturnFavor),length(AnswermotivatedToReturnFavor,L73),write(ENC),write('\tSTATIC FUNCTION	motivatedToReturnFavor\t'),write(L73),write('\n').
run_motivatedToReturnFavor :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	motivatedToReturnFavor\t0\n').
run_successfulCoercion :-
	encoding(ENC),
	setof((A,B,C,D,E), successfulCoercion(A,B,C,D,E),AnswersuccessfulCoercion),length(AnswersuccessfulCoercion,L74),write(ENC),write('\tSTATIC FUNCTION	successfulCoercion\t'),write(L74),write('\n').
run_successfulCoercion :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	successfulCoercion\t0\n').
run_hiddenAgenda :-
	encoding(ENC),
	setof((A,B,C,D,E), hiddenAgenda(A,B,C,D,E),AnswerhiddenAgenda),length(AnswerhiddenAgenda,L75),write(ENC),write('\tSTATIC FUNCTION	hiddenAgenda\t'),write(L75),write('\n').
run_hiddenAgenda :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	hiddenAgenda\t0\n').
run_betrayal :-
	encoding(ENC),
	setof((A,B,C,D,E,F,G), betrayal(A,B,C,D,E,F,G),Answerbetrayal),length(Answerbetrayal,L76),write(ENC),write('\tSTATIC FUNCTION	betrayal\t'),write(L76),write('\n').
run_betrayal :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	betrayal\t0\n').
run_flashback :-
	encoding(ENC),
	setof((A,B), flashback(A,B),Answerflashback),length(Answerflashback,L77),write(ENC),write('\tSTATIC FUNCTION	flashback\t'),write(L77),write('\n').
run_flashback :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	flashback\t0\n').
run_flashforward :-
	encoding(ENC),
	setof((A,B), flashforward(A,B),Answerflashforward),length(Answerflashforward,L78),write(ENC),write('\tSTATIC FUNCTION	flashforward\t'),write(L78),write('\n').
run_flashforward :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	flashforward\t0\n').
run_suspense :-
	encoding(ENC),
	setof((A,B,C,D,E), suspense(A,B,C,D,E),Answersuspense),length(Answersuspense,L79),write(ENC),write('\tSTATIC FUNCTION	suspense\t'),write(L79),write('\n').
run_suspense :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	suspense\t0\n').
run_mystery :-
	encoding(ENC),
	setof((A,B,C), mystery(A,B,C),Answermystery),length(Answermystery,L80),write(ENC),write('\tSTATIC FUNCTION	mystery\t'),write(L80),write('\n').
run_mystery :-
	encoding(ENC),
	write(ENC),write('\tSTATIC FUNCTION	mystery\t0\n').
runStaticGoals :-
	run_gain,
	run_loss,
	run_negativeResolution,
	run_positiveResolution,
	run_complexPositive,
	run_complexNegative,
	run_hiddenBlessing,
	run_positiveTradeoff,
	run_negativeTradeoff,
	run_mixedBlessing,
	run_promise,
	run_threat,
	run_promiseFulfilled,
	run_threatFulfilled,
	run_promiseBroken,
	run_threatAvoided,
	run_partialResolution,
	run_compoundedTransition,
	run_goalDeclared,
	run_desireToAid,
	run_desireToHarm,
	run_explicitMotivation,
	run_problem,
	run_changeOfMind,
	run_goalEnablement,
	run_goalObstacle,
	run_goalSuccessExpected,
	run_goalFailureExpected,
	run_goalAvoidance,
	run_goalPreemption,
	run_perseverance,
	run_success,
	run_failure,
	run_deliberateAid,
	run_deliberateHarm,
	run_unintendedAid,
	run_unintendedHarm,
	run_backfireType1,
	run_backfireType2,
	run_lostOpportunity,
	run_goodSideEffect,
	run_badSideEffect,
	run_recovery,
	run_peripeteia,
	run_goalSubstitution,
	run_failureGivingUp,
	run_noir,
	run_obviatedPlan,
	run_wastedEffortIrony,
	run_mistakenBelief,
	run_violatedExpectation,
	run_surprise,
	run_anagroisis,
	run_potentialContradiction,
	run_contradictoryBeliefs,
	run_mistakenSatisfaction,
	run_dilemmaType1,
	run_dilemmaType2,
	run_goalPrioritization,
	run_selfishAct,
	run_selflessAct,
	run_deliberateAssistance,
	run_commonlyPursuedGoal,
	run_tandemAttempts,
	run_conflictType1,
	run_conflictType2,
	run_giftOfTheMagiIrony,
	run_persuasion,
	run_deception,
	run_unintendedPersuasion,
	run_mutualDeception,
	run_motivatedToRevenge,
	run_motivatedToReturnFavor,
	run_successfulCoercion,
	run_hiddenAgenda,
	run_betrayal,
	run_flashback,
	run_flashforward,
	run_suspense,
	run_mystery,
	halt.
