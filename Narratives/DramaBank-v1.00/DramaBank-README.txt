README for DramaBank, v1.00
[anonymized]

=========
 SUMMARY
=========
DramaBank consists of a corpus of stories manually encoded for
narrative discourse relations. Specifically, annotators identified
coreferent agents, events, statives, and temporal relationships, as
well as further agentive content: goals, plans, beliefs, and affectual
impacts.  These annotations represent "theory-of-mind" and
"situation model" readings of their texts (from literary theory and
cognitive psychology, respectively), in that agents have distinct
belief and intention frames which are connected in a single connected
structure (a semantic network) representing the entire discourse.  For
further details on the Story Intention Graph model, see the paper you
are now reviewing.

==============
 DATA FORMATS
==============
There are 70 encodings in DramaBank in the VGL/ directory. These are
data files readable by the Scheherazade annotation tool [anonymized
URL].  Simply load each file from the Scheherazade splash screen to
see a graphical annotation interface from which the encodings can be
examined and altered.

Scheherazade can also be treated as a library capable of creating,
editing, loading and saving SIG encodings.  See the Scheherazade
distribution for a Javadoc and a code example of using the
Scheherazade API.

An alternate version of 66 of the 70 encodings exists in the
assertions/ directory (four were omitted for technical reasons).
These Prolog files, in standard Prolog format, serialize their
respective encodings as first-order predicates: Unary relations
describe attributes of nodes (coreferent entities); binary relations
describe relations.  See also SIG-closure-rules.prolog, which
describes the logical closure applied to each SIG encoding in the
analogy detection process described in the above paper.

The file names consist of a four-character annotator code (e.g., A114)
and the unique story ID.

==========
 MANIFEST
==========
See associated paper for a list of the stories encoded, and for each
story, the number of encodings collected and the coverage of the text
provided by its encodings.

=================
 RELEASE HISTORY
=================
## 1.00 (2011-09-19)
-- Initial release.






