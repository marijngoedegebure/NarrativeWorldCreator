/**************************************************************************
 * 
 * Enums.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 * 
 **************************************************************************
 * 
 * A collection of many enums.
 *
 *************************************************************************/

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Common;

namespace Semantics.Utilities
{

    #region Enum: Message
    /// <summary>
    /// A message to indicate whether certain actions have been executed successfully.
    /// </summary>
    public enum Message
    {
        /// <summary>
        /// The initialization has been successful.
        /// </summary>
        InitializationSuccess,

        /// <summary>
        /// The initialization has failed.
        /// </summary>
        InitializationFail,

        /// <summary>
        /// The initialization has already been done.
        /// </summary>
        AlreadyInitialized,

        /// <summary>
        /// The uninitialization has been successful.
        /// </summary>
        UninitializationSuccess,

        /// <summary>
        /// The uninitialization has failed.
        /// </summary>
        UninitializationFail,

        /// <summary>
        /// The creation has been successful.
        /// </summary>
        CreationSuccess,

        /// <summary>
        /// The creation has failed.
        /// </summary>
        CreationFail,

        /// <summary>
        /// The switch has been successful.
        /// </summary>
        SwitchSuccess,

        /// <summary>
        /// The switch has failed.
        /// </summary>
        SwitchFail,

        /// <summary>
        /// The removal has been successful.
        /// </summary>
        RemovalSuccess,

        /// <summary>
        /// The removal has failed.
        /// </summary>
        RemovalFail,

        /// <summary>
        /// Saving has been successful.
        /// </summary>
        SaveSuccess,

        /// <summary>
        /// Saving has failed.
        /// </summary>
        SaveFail,

        /// <summary>
        /// The relation has been succesfully created.
        /// </summary>
        RelationSuccess,

        /// <summary>
        /// The creation of the relation failed.
        /// </summary>
        RelationFail,

        /// <summary>
        /// The relation exists already.
        /// </summary>
        RelationExistsAlready,

        /// <summary>
        /// It is not allowed to remove the node.
        /// </summary>
        RemovalNotAllowed
    }
    #endregion Enum: Message

    #region Enum: ActorTarget
    /// <summary>
    /// Actor or target.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ActorTarget
    {
        /// <summary>
        /// The actor.
        /// </summary>
        Actor = 0,

        /// <summary>
        /// The target.
        /// </summary>
        Target = 1
    }
    #endregion Enum: ActorTarget

    #region Enum: ActorTargetArtifact
    /// <summary>
    /// Actor, target, or artifact.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ActorTargetArtifact
    {
        /// <summary>
        /// The actor.
        /// </summary>
        Actor = 0,

        /// <summary>
        /// The target.
        /// </summary>
        Target = 1,

        /// <summary>
        /// The artifact.
        /// </summary>
        Artifact = 2
    }
    #endregion Enum: ActorTargetArtifact

    #region Enum: ActorTargetArtifactReference
    /// <summary>
    /// Actor, target, artifact, or reference.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ActorTargetArtifactReference
    {
        /// <summary>
        /// The actor.
        /// </summary>
        Actor = 0,

        /// <summary>
        /// The target.
        /// </summary>
        Target = 1,

        /// <summary>
        /// The artifact.
        /// </summary>
        Artifact = 2,

        /// <summary>
        /// A reference.
        /// </summary>
        Reference = 3
    }
    #endregion Enum: ActorTargetArtifactReference

    #region Enum: AttributeType
    /// <summary>
    /// The type of an attribute.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum AttributeType
    {
        /// <summary>
        /// The value of the attribute can be changed.
        /// </summary>
        Mutable = 0,

        /// <summary>
        /// The value of the attribute is constant and cannot be changed.
        /// </summary>
        Constant = 1,

        /// <summary>
        /// The value of the attribute is dependent on other attributes.
        /// </summary>
        Dependent = 2
    }
    #endregion Enum: AttributeType

    #region Enum: BinaryOperator
    /// <summary>
    /// Binary operators from the set theory.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum BinaryOperator
    {
        /// <summary>
        /// Union of the sets A and B, denoted A ∪ B, is the set of all objects that are a member of A, or B, or both. The union of {1, 2, 3} and {2, 3, 4} is the set {1, 2, 3, 4}.
        /// </summary>
        Union = 0,

        /// <summary>
        /// Intersection of the sets A and B, denoted A ∩ B, is the set of all objects that are members of both A and B. The intersection of {1, 2, 3} and {2, 3, 4} is the set {2, 3}.
        /// </summary>
        Intersection = 1,

        /// <summary>
        /// Set difference of U and A, denoted U \ A is the set of all members of U that are not members of A. The set difference {1,2,3} \ {2,3,4} is {1} , while, conversely, the set difference {2,3,4} \ {1,2,3} is {4} . When A is a subset of U, the set difference U \ A is also called the complement of A in U.
        /// </summary>
        Complement = 2,

        /// <summary>
        /// Symmetric difference of sets A and B is the set of all objects that are a member of exactly one of A and B (elements which are in one of the sets, but not in both). For instance, for the sets {1,2,3} and {2,3,4} , the symmetric difference set is {1,4} . It is the set difference of the union and the intersection, (A ∪ B) \ (A ∩ B).
        /// </summary>
        SymmetricDifference = 3
    }
    #endregion Enum: BinaryOperator

    #region Enum: BooleanType
    /// <summary>
    /// The type of boolean.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum BooleanType
    {
        /// <summary>
        /// True.
        /// </summary>
        True = 0,

        /// <summary>
        /// False.
        /// </summary>
        False = 1
    }
    #endregion Enum: BooleanType

    #region Enum: Cardinality
    /// <summary>
    /// The cardinality between source and target.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Cardinality
    {
        /// <summary>
        /// The source can be related to multiple targets of the same type, and the target to multiple sources of the same type.
        /// </summary>
        ManyToMany = 0,

        /// <summary>
        /// The target can be related to multiple sources of the same type, but each source can only be related to one target of a type.
        /// </summary>
        ManyToOne = 1,

        /// <summary>
        /// The source can be related to multiple targets of the same type, but each target can only be related to one source of a type.
        /// </summary>
        OneToMany = 2,

        /// <summary>
        /// Source and target can only be related to one of a type.
        /// </summary>
        OneToOne = 3
    }
    #endregion Enum: Cardinality

    #region Enum: Composition
    /// <summary>
    /// The composition of a mixture.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Composition
    {
        /// <summary>
        /// A heterogeneous mixture is a type of mixture in which the composition can easily be identified, as there are two or more phases present.
        /// </summary>
        Heterogeneous = 1,

        /// <summary>
        /// A homogeneous mixture is a type of mixture in which the composition is uniform.
        /// </summary>
        Homogeneous = 0
    }
    #endregion Enum: Composition

    #region Enum: CountType
    /// <summary>
    /// Indicates what should be counted.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CountType
    {
        /// <summary>
        /// Count everything.
        /// </summary>
        Everything = 0,

        /// <summary>
        /// Count the spaces.
        /// </summary>
        Spaces = 1,

        /// <summary>
        /// Count the space items.
        /// </summary>
        SpaceItems = 2,

        /// <summary>
        /// Count the parts
        /// </summary>
        Parts = 3,

        /// <summary>
        /// Count the covers.
        /// </summary>
        Covers = 4,

        /// <summary>
        /// Count the connections.
        /// </summary>
        Connections = 5,

        /// <summary>
        /// Count the layers.
        /// </summary>
        Layers = 6
    }
    #endregion Enum: CountType

    #region Enum: CreationRelationshipSourceTarget
    /// <summary>
    /// Indicates who should be the source and target of the relationship that has to be established on the creation of an entity.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CreationRelationshipSourceTarget
    {
        /// <summary>
        /// The new entity should be the source of the relationship, and the actor of the event the target of the relationhip.
        /// </summary>
        RelateEntityAsSourceAndActorAsTarget = 0,

        /// <summary>
        /// The new entity should be the source of the relationship, and the target of the event the target of the relationhip.
        /// </summary>
        RelateEntityAsSourceAndTargetAsTarget = 1,

        /// <summary>
        /// The new entity should be the target of the relationship, and the actor of the event the source of the relationhip.
        /// </summary>
        RelateEntityAsTargetAndActorAsSource = 2,

        /// <summary>
        /// The new entity should be the target of the relationship, and the target of the event the source of the relationhip.
        /// </summary>
        RelateEntityAsTargetAndTargetAsSource = 3
    }
    #endregion Enum: CreationRelationshipSourceTarget

    #region Enum: DeletionType
    /// <summary>
    /// The type of a deletion.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DeletionType
    {
        /// <summary>
        /// Delete all occurences of the entity.
        /// </summary>
        DeleteAll = 0,

        /// <summary>
        /// Delete the right number of entities, or the right amount of the entity, that is defined at the quantity.
        /// </summary>
        DefinedAtQuantity = 1,
    }
    #endregion Enum: DeletionType

    #region Enum: DesiredValueType
    /// <summary>
    /// The desired value type.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DesiredValueType
    {
        /// <summary>
        /// The value should be equal to a particular value.
        /// </summary>
        Equal = 0,

        /// <summary>
        /// The value should not be equal to a particular value.
        /// </summary>
        NotEqual = 1,

        /// <summary>
        /// The value should be greater than a particular value.
        /// </summary>
        Greater = 2,

        /// <summary>
        /// The value should be greater than or equal to a particular value.
        /// </summary>
        GreaterOrEqual = 3,

        /// <summary>
        /// The value should be lower than a particular value.
        /// </summary>
        Lower = 4,

        /// <summary>
        /// The value should be lower than or equal to a particular value.
        /// </summary>
        LowerOrEqual = 5,

        /// <summary>
        /// The value should be increased.
        /// </summary>
        Increase = 6,

        /// <summary>
        /// The value should be increased by at least a particular value.
        /// </summary>
        IncreaseByAtLeast = 7,

        /// <summary>
        /// The value should be increased by at most a particular value.
        /// </summary>
        IncreaseByAtMost = 8,

        /// <summary>
        /// The value should be decreased.
        /// </summary>
        Decrease = 9,

        /// <summary>
        /// The value should be decreased by at least a particular value.
        /// </summary>
        DecreaseByAtLeast = 10,

        /// <summary>
        /// The value should be decreased by at most a particular value.
        /// </summary>
        DecreaseByAtMost = 11,

        /// <summary>
        /// The value should be between two values.
        /// </summary>
        Between = 12,

        /// <summary>
        /// The value should be increased by at least a particular value, but at most by another value.
        /// </summary>
        IncreaseBetween = 13,

        /// <summary>
        /// The value should be decreased by at least a particular value, but at most by another value.
        /// </summary>
        DecreaseBetween = 14
    }
    #endregion Enum: DesiredValueType

    #region Enum: Destination
    /// <summary>
    /// The destination of a created entity.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Destination
    {
        /// <summary>
        /// The created entity is just added to the world.
        /// </summary>
        World = 0,

        /// <summary>
        /// The created entity is added as an item of the actor.
        /// </summary>
        ActorItems = 1,

        /// <summary>
        /// The created entity is added as a part of the actor.
        /// </summary>
        ActorParts = 2,

        /// <summary>
        /// The created entity is added as a cover of the actor.
        /// </summary>
        ActorCovers = 3,

        /// <summary>
        /// The created entity is added as a layer of the actor.
        /// </summary>
        ActorLayers = 4,

        /// <summary>
        /// The created entity is added as a substance of the actor.
        /// </summary>
        ActorSubstances = 5,

        /// <summary>
        /// The created entity is added as an element of the actor.
        /// </summary>
        ActorElements = 6,

        /// <summary>
        /// The created entity is added as a space of the actor.
        /// </summary>
        ActorSpaces = 7,
        
        /// <summary>
        /// The created entity is added as an item of the target.
        /// </summary>
        TargetItems = 8,

        /// <summary>
        /// The created entity is added as a part of the target.
        /// </summary>
        TargetParts = 9,

        /// <summary>
        /// The created entity is added as a cover of the target.
        /// </summary>
        TargetCovers = 10,

        /// <summary>
        /// The created entity is added as a layer of the target.
        /// </summary>
        TargetLayers = 11,

        /// <summary>
        /// The created entity is added as a substance of the target.
        /// </summary>
        TargetSubstances = 12,

        /// <summary>
        /// The created entity is added as an element of the target.
        /// </summary>
        TargetElements = 13,

        /// <summary>
        /// The created entity is added as a space of the target.
        /// </summary>
        TargetSpaces = 14
    }
    #endregion Enum: Destination

    #region Enum: DurationType
    /// <summary>
    /// The type of duration.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DurationType
    {
        /// <summary>
        /// The duration is an infinite amount of time.
        /// </summary>
        Infinite = 0,

        /// <summary>
        /// The duration is an infinite amount of time, as long as the requirements are satisfied.
        /// </summary>
        InfiniteDependentOnRequirements = 1,

        /// <summary>
        /// The duration is a fixed amount of time.
        /// </summary>
        Fixed = 2,

        /// <summary>
        /// The duration is a fixed amount of time, as long as the requirements are satisfied.
        /// </summary>
        FixedDependentOnRequirements = 3
    }
    #endregion Enum: DurationType

    #region Enum: ElementBlock
    /// <summary>
    /// The block of an element.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ElementBlock
    {
        S = 0,
        P = 1,
        D = 2,
        F = 3,
        G = 4
    }
    #endregion Enum: ElementBlock

    #region Enum: ElementGroup
    /// <summary>
    /// The group of an element.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ElementGroup
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7,
        Nine = 8,
        Ten = 9,
        Eleven = 10,
        Twelve = 11,
        Thirteen = 12,
        Fourteen = 13,
        Fifteen = 14,
        Sixteen = 15,
        Seventeen = 16,
        Eighteen = 17
    }
    #endregion Enum: ElementGroup

    #region Enum: ElementPeriod
    /// <summary>
    /// The period of an element.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ElementPeriod
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7
    }
    #endregion Enum: ElementPeriod

    #region Enum: ElementType
    /// <summary>
    /// The type of an element.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ElementType
    {
        /// <summary>
        /// Metal.
        /// </summary>
        Metal = 1,

        /// <summary>
        /// Metalloid.
        /// </summary>
        Metalloid = 2,

        /// <summary>
        /// Non-metal.
        /// </summary>
        Nonmetal = 0
    }
    #endregion Enum: ElementType

    #region Enum: EqualitySign
    /// <summary>
    /// The sign of an equality
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EqualitySign
    {
        /// <summary>
        /// The value should be equal to the other value
        /// </summary>
        Equal = 0,

        /// <summary>
        /// The value should not be equal to the other value
        /// </summary>
        NotEqual = 1
    }
    #endregion Enum: EqualitySign

    #region Enum: EqualitySignExtended
    /// <summary>
    /// The sign of an equality.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EqualitySignExtended
    {
        /// <summary>
        /// The value should be equal to the other value.
        /// </summary>
        Equal = 0,

        /// <summary>
        /// The value should not be equal to the other value.
        /// </summary>
        NotEqual = 1,

        /// <summary>
        /// The value should be greater than the other value.
        /// </summary>
        Greater = 2,

        /// <summary>
        /// The value should be greater than or equal to the other value.
        /// </summary>
        GreaterOrEqual = 3,

        /// <summary>
        /// The value should be lower then the other value.
        /// </summary>
        Lower = 4,

        /// <summary>
        /// The value should be lower than or equal to the other value.
        /// </summary>
        LowerOrEqual = 5
    }
    #endregion Enum: EqualitySignExtended

    #region Enum: EqualitySignExtendedDual
    /// <summary>
    /// The sign of a dual equality.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EqualitySignExtendedDual
    {
        /// <summary>
        /// The value should be equal to the other value.
        /// </summary>
        Equal = 0,

        /// <summary>
        /// The value should not be equal to the other value.
        /// </summary>
        NotEqual = 1,

        /// <summary>
        /// The value should be greater than the other value.
        /// </summary>
        Greater = 2,

        /// <summary>
        /// The value should be greater than or equal to the other value.
        /// </summary>
        GreaterOrEqual = 3,

        /// <summary>
        /// The value should be lower then the other value.
        /// </summary>
        Lower = 4,

        /// <summary>
        /// The value should be lower than or equal to the other value.
        /// </summary>
        LowerOrEqual = 5,

        /// <summary>
        /// The value should be between two values.
        /// </summary>
        Between = 6,

        /// <summary>
        /// The value should not be between two values.
        /// </summary>
        NotBetween = 7
    }
    #endregion Enum: EqualitySignExtendedDual

    #region Enum: EventBehavior
    /// <summary>
    /// The behavior of an event indicates when it should be executed.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EventBehavior
    {
        /// <summary>
        /// The (effects of the) event will be triggered automatically when all conditions have been satisfied.
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// The (effects of the) event will be triggered when manually requested and the conditions have been satisfied.
        /// </summary>
        Manual = 1
    }
    #endregion Enum: EventBehavior

    #region Enum: EventState
    /// <summary>
    /// The state of an event.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EventState
    {
        /// <summary>
        /// The event starts.
        /// </summary>
        Starts = 0,

        /// <summary>
        /// The event stops.
        /// </summary>
        Stops = 1
    }
    #endregion Enum: EventState

    #region Enum: FrequencyType
    /// <summary>
    /// The type of frequency.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum FrequencyType
    {
        /// <summary>
        /// The frequency is a fixed number.
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// The frequency is infinite.
        /// </summary>
        Infinite = 1
    }
    #endregion Enum: FrequencyType

    #region Enum: Function
    /// <summary>
    /// A function to perform.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Function
    {
        /// <summary>
        /// The sine function.
        /// </summary>
        Sine = 0,

        /// <summary>
        /// The cosine function.
        /// </summary>
        Cosine = 1,

        /// <summary>
        /// The tangent function.
        /// </summary>
        Tangent = 2,

        /// <summary>
        /// The asine function.
        /// </summary>
        Asine = 3,

        /// <summary>
        /// The acosine function.
        /// </summary>
        Acosine = 4,

        /// <summary>
        /// The atangent function.
        /// </summary>
        Atangent = 5,

        /// <summary>
        /// The square root.
        /// </summary>
        SquareRoot = 6,

        /// <summary>
        /// Round off to the ceiling.
        /// </summary>
        Ceiling = 7,

        /// <summary>
        /// Round off to the floor.
        /// </summary>
        Floor = 8,

        /// <summary>
        /// The natural logarithm.
        /// </summary>
        NaturalLogarithm = 9,

        /// <summary>
        /// The tenth logarithm.
        /// </summary>
        Log10 = 10,

        /// <summary>
        /// The exponent function.
        /// </summary>
        Exponent = 11,

        /// <summary>
        /// The absolute value.
        /// </summary>
        Absolute = 12,

        /// <summary>
        /// The rounded value.
        /// </summary>
        Round = 13
    }
    #endregion Enum: Function

    #region Enum: LogicalOperator
    /// <summary>
    /// The logical operators.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum LogicalOperator
    {
        /// <summary>
        /// There should be no operator.
        /// </summary>
        None = 0,

        /// <summary>
        /// The AND operator.
        /// </summary>
        And = 1,

        /// <summary>
        /// The OR operator.
        /// </summary>
        Or = 2
    }
    #endregion Enum: LogicalOperator

    #region Enum: MetalType
    /// <summary>
    /// The type of a metal.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum MetalType
    {
        /// <summary>
        /// In chemistry, the term base metal is used informally to refer to a metal that oxidizes or corrodes relatively easily.
        /// </summary>
        Base = 0,
        
        /// <summary>
        /// The term "ferrous" is derived from the Latin word meaning "containing iron".
        /// </summary>
        Ferrous = 1,

        /// <summary>
        /// Noble metals are metals that are resistant to corrosion or oxidation, unlike most base metals.
        /// </summary>
        Noble = 2,

        /// <summary>
        /// A precious metal is a rare metallic chemical element of high economic value.
        /// </summary>
        Precious = 3
    }
    #endregion Enum: MetalType

    #region Enum: MixtureType
    /// <summary>
    /// The type of a mixture.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum MixtureType
    {
        /// <summary>
        /// An alloy is a partial or complete solid solution of one or more elements in a metallic matrix.
        /// </summary>
        Alloy = 3,

        /// <summary>
        /// A colloid is a substance microscopically dispersed evenly throughout another one.
        /// </summary>
        Colloid = 2,

        /// <summary>
        /// A solution is a homogeneous mixture composed of two or more substances.
        /// </summary>
        Solution = 0,

        /// <summary>
        /// A suspension is a heterogeneous fluid containing solid particles that are sufficiently large for sedimentation.
        /// </summary>
        Suspension = 1
    }
    #endregion Enum: MixtureType

    #region Enum: Necessity
    /// <summary>
    /// The necessity indicates whether something is mandatory or optional.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Necessity
    {
        /// <summary>
        /// It is mandatory.
        /// </summary>
        Mandatory,

        /// <summary>
        /// It is optional.
        /// </summary>
        Optional
    }
    #endregion Enum: Necessity

    #region Enum: Operator
    /// <summary>
    /// Operators.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Operator
    {
        /// <summary>
        /// An addition.
        /// </summary>
        Addition = 0,

        /// <summary>
        /// A subtraction.
        /// </summary>
        Subtraction = 1,

        /// <summary>
        /// A multiplication.
        /// </summary>
        Multiplication = 2,

        /// <summary>
        /// A division.
        /// </summary>
        Division = 3,

        /// <summary>
        /// A power.
        /// </summary>
        Power = 4
    }
    #endregion Enum: Operator

    #region Enum: OwnerType
    /// <summary>
    /// The type of an owner.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum OwnerType
    {
        /// <summary>
        /// The owner of an item is a particular space in which it is an item.
        /// </summary>
        SpaceOfItem = 0,

        /// <summary>
        /// The owner of a part is a particular whole (tangible object) of which it is a part.
        /// </summary>
        WholeOfPart = 1,

        /// <summary>
        /// The owner of a space is a particular physical object of which it is a space.
        /// </summary>
        PhysicalObjectOfSpace = 2,

        /// <summary>
        /// The owner of a substance is a particular compound of which it is a substance.
        /// </summary>
        CompoundOfSubstance = 3,

        /// <summary>
        /// The owner of a substance is a particular mixture of which it is a substance.
        /// </summary>
        MixtureOfSubstance = 4,

        /// <summary>
        /// The owner of a cover is a particular covered object (tangible object) of which it is a cover.
        /// </summary>
        CoveredObjectOfCover = 5,

        /// <summary>
        /// The owner of a layer is a particular applicant (tangible object) of which it is a layer.
        /// </summary>
        ApplicantOfLayer = 6,

        /// <summary>
        /// The owner of matter is a particular tangible object with that matter.
        /// </summary>
        TangibleObjectOfMatter = 7,

        /// <summary>
        /// The owner of matter is a particular space in which it is tangible matter.
        /// </summary>
        SpaceOfMatter = 8
    }
    #endregion Enum: OwnerType

    #region Enum: Prefix
    /// <summary>
    /// The prefix of a unit.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Prefix
    {
        Yocto = 1,
        Zepto = 2,
        Atto = 3,
        Femto = 4,
        Pico = 5,
        Nano = 6,
        Micro = 7,
        Milli = 8,
        Centi = 9,
        Deci = 10,
        None = 0,
        Deca = 11,
        Hecto = 12,
        Kilo = 13,
        Mega = 14,
        Giga = 15,
        Tera = 16,
        Peta = 17,
        Exa = 18,
        Zetta = 19,
        Yotta = 20
    }
    #endregion Enum: Prefix

    #region Enum: Presence
    /// <summary>
    /// Indicates whether items in a space are attached to that space (so move along), or just present in that space.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum Presence
    {
        /// <summary>
        /// It is attached.
        /// </summary>
        Attached,

        /// <summary>
        /// It is present.
        /// </summary>
        Present
    }
    #endregion Enum: Presence

    #region Enum: RangeType
    /// <summary>
    /// The type of a range.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum RangeType
    {
        /// <summary>
        /// Only the specified entities are included in the range.
        /// </summary>
        EntitiesOnly = 0,

        /// <summary>
        /// All entities within the specified radius are included in the range.
        /// </summary>
        RadiusOnly = 1,

        /// <summary>
        /// All specified entities within the specified radius are included in the range.
        /// </summary>
        EntitiesInRadius = 2
    }
    #endregion Enum: RangeType

    #region Enum: SelectionType
    /// <summary>
    /// The type of a selection.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SelectionType
    {
        /// <summary>
        /// All should be selected.
        /// </summary>
        All = 0,

        /// <summary>
        /// Only the nearest ones should be selected.
        /// </summary>
        Nearest = 1,

        /// <summary>
        /// Only the farthest ones should be selected.
        /// </summary>
        Farthest = 2,

        /// <summary>
        /// Random ones should be selected.
        /// </summary>
        Random = 3
    }
    #endregion Enum: SelectionType

    #region Enum: SourceTarget
    /// <summary>
    /// Indicates a source or target.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SourceTarget
    {
        /// <summary>
        /// Source.
        /// </summary>
        Source = 0,

        /// <summary>
        /// Target.
        /// </summary>
        Target = 1
    }
    #endregion Enum: SourceTarget

    #region Enum: SpaceType
    /// <summary>
    /// The type of a space.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SpaceType
    {
        /// <summary>
        /// The space is open for everyone; everything can be taken.
        /// </summary>
        Open = 0,

        /// <summary>
        /// The space is closed for anyone but the owner; nothing can be taken.
        /// </summary>
        Closed = 1
    }
    #endregion Enum: SpaceType

    #region Enum: SpecialAttributes
    /// <summary>
    /// Indicates the special attributes.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SpecialAttributes
    {
        /// <summary>
        /// Position.
        /// </summary>
        Position = 0,

        /// <summary>
        /// Rotation.
        /// </summary>
        Rotation = 1,

        /// <summary>
        /// Scale.
        /// </summary>
        Scale = 2
    }
    #endregion Enum: SpecialAttributes

    #region Enum: SpecialUnitCategories
    /// <summary>
    /// Indicates the special unit categories.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SpecialUnitCategories
    {
        /// <summary>
        /// Quantity.
        /// </summary>
        Quantity = 0,

        /// <summary>
        /// Distance.
        /// </summary>
        Distance = 1,

        /// <summary>
        /// Time.
        /// </summary>
        Time = 2
    }
    #endregion Enum: SpecialUnitCategories

    #region Enum: StateOfMatter
    /// <summary>
    /// The state of matter.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum StateOfMatter
    {
        /// <summary>
        /// Gas is that state in which the molecules are comparatively separated and intermolecular attractions have relatively little effect on their respective motions.
        /// </summary>
        Gas = 2,

        /// <summary>
        /// Liquid is the state in which intermolecular attractions keep molecules in proximity, but do not keep the molecules in fixed relationships.
        /// </summary>
        Liquid = 0,

        /// <summary>
        /// Plasma is a highly ionized gas that occurs at high temperatures.
        /// </summary>
        Plasma = 3,

        /// <summary>
        /// Solid is the state in which intermolecular attractions keep the molecules in fixed spatial relationships.
        /// </summary>
        Solid = 1
    }
    #endregion Enum: StateOfMatter

    #region Enum: StorageType
    /// <summary>
    /// The storage type for a valued space.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum StorageType
    {
        /// <summary>
        /// The valued space can store an unlimited number of items.
        /// </summary>
        Unlimited = 0,

        /// <summary>
        /// The valued space has a maximum on the number of items it can store.
        /// </summary>
        MaximumNumberOfItems = 1,

        /// <summary>
        /// The maximum storage is limited by the capacity of the valued space.
        /// </summary>
        Capacity = 2,

        /// <summary>
        /// The maximum storage is limited by the number of items, or the capacity of the valued space, whichever comes first.
        /// </summary>
        MaximumNumberOfItemsAndCapacity = 3
    }
    #endregion Enum: StorageType

    #region Enum: TimeType
    /// <summary>
    /// The type of time.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum TimeType
    {
        /// <summary>
        /// The time is discrete.
        /// </summary>
        Discrete = 0,

        /// <summary>
        /// The time is continuous.
        /// </summary>
        Continuous = 1
    }
    #endregion Enum: TimeType

    #region Enum: TransferDirection
    /// <summary>
    /// The direction of a transfer.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum TransferDirection
    {
        /// <summary>
        /// The transfer takes place from the actor to the target.
        /// </summary>
        ActorToTarget = 0,

        /// <summary>
        /// The transfer takes place from the target to the actor.
        /// </summary>
        TargetToActor = 1,

        /// <summary>
        /// The transfer takes place from one space of the actor to another.
        /// </summary>
        ActorToActor = 2,

        /// <summary>
        /// The transfer takes place from one space of the target to another.
        /// </summary>
        TargetToTarget = 3
    }
    #endregion Enum: TransferDirection

    #region Enum: TransferType
    /// <summary>
    /// The type of a transfer.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum TransferType
    {
        /// <summary>
        /// Items are transferred.
        /// </summary>
        Items = 0,

        /// <summary>
        /// The actor is transferred to a space of the target.
        /// </summary>
        ActorToTarget = 1,

        /// <summary>
        /// The target is transferred to a space of the actor.
        /// </summary>
        TargetToActor = 2
    }
    #endregion Enum: TransferType

    #region Enum: ValueChangeType
    /// <summary>
    /// The type of a change.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ValueChangeType
    {
        /// <summary>
        /// A value should be increased.
        /// </summary>
        Increase = 0,

        /// <summary>
        /// A value should be decreased.
        /// </summary>
        Decrease = 1,

        /// <summary>
        /// A value should be equated to a value.
        /// </summary>
        Equate = 2,

        /// <summary>
        /// A value should be multiplied by a value.
        /// </summary>
        Multiply = 3,

        /// <summary>
        /// A value should be divided by a value.
        /// </summary>
        Divide = 4
    }
    #endregion Enum: ValueChangeType

    #region Enum: ValueType
    /// <summary>
    /// The type of a value.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ValueType
    {
        /// <summary>
        /// The value is defined as a numerical value.
        /// </summary>
        Numerical = 0,

        /// <summary>
        /// The value is defined as a boolean.
        /// </summary>
        Boolean = 1,

        /// <summary>
        /// The value is defined as a string.
        /// </summary>
        String = 2,

        /// <summary>
        /// The value is defined as a vector.
        /// </summary>
        Vector = 3,

        /// <summary>
        /// The value is defined as a term.
        /// </summary>
        Term = 4
    }
    #endregion Enum: ValueType

    #region Enum: VariableType
    /// <summary>
    /// The type of a variable.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum VariableType
    {
        /// <summary>
        /// The variable has a fixed value.
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// The value of the variable is taken from an attribute.
        /// </summary>
        Attribute = 1,

        /// <summary>
        /// The value of the variable is a particular count.
        /// </summary>
        Count = 2,

        /// <summary>
        /// The value of the variable is a particular quantity.
        /// </summary>
        Quantity = 3,

        /// <summary>
        /// The value of the variable is the sum of attribute values.
        /// </summary>
        Sum = 4,

        /// <summary>
        /// The value of the variable is the distance between actor and target.
        /// </summary>
        Distance = 5,

        /// <summary>
        /// The variable requires manual input.
        /// </summary>
        RequiresManualInput = 6
    }
    #endregion Enum: VariableType

    #region Class: LocalizedEnumConverter
    /// <summary>
    /// Defines a type converter for enum types defined in this project.
    /// http://www.codeproject.com/KB/cs/LocalizingEnums.aspx
    /// </summary>
    class LocalizedEnumConverter : ResourceEnumConverter
    {
        /// <summary>
        /// Create a new instance of the converter using translations from the given resource manager.
        /// </summary>
        public LocalizedEnumConverter(Type type)
            : base(type, Enums.ResourceManager)
        {
        }
    }
    #endregion Class: LocalizedEnumConverter

}