export const ExerciseType = {
  Cardio: 'Cardio',
  Strength: 'Strength',
  Flexibility: 'Flexibility',
} as const;

export type ExerciseType = (typeof ExerciseType)[keyof typeof ExerciseType];