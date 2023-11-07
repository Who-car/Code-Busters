export interface Question {
    id: number;
    question: string | undefined;
    description: null | string;
    answers: {
      answer_a: string | null;
      answer_b: string | null;
      answer_c: string | null;
      answer_d: string | null;
      answer_e: string | null;
      answer_f: string | null;
    };
    multiple_correct_answers: "true" | "false";
    correct_answers: {
      answer_a_correct: "true" | "false";
      answer_b_correct: "true" | "false";
      answer_c_correct: "true" | "false";
      answer_d_correct: "true" | "false";
      answer_e_correct: "true" | "false";
      answer_f_correct: "true" | "false";
    };
    correct_answer: "answer_a" | "answer_b" | "answer_c" | "answer_d" | "answer_e" | "answer_f";
    explanation: null | string;
    tip: null | string;
    tags: {
      name: string;
    }[];
    category: string;
    difficulty: "Easy" | "Medium" | "Hard";
}
  