using System;
using System.Collections.Generic;
using TCDSwift;

public class SSA
{
  public IRGraph DoSSAOptimizations(IRGraph graph) {
    // convert into SSA form
    IRGraph ssa_form = this.ConvertIntoSSAForm(graph);

    // do optimizations (constant propagation, dead code elimination)
    IRGraph no_dead_code = this.DeadCodeElimination(ssa_form);
    IRGraph constants_propagated = this.ConstantPropagation(no_dead_code);

    // use Briggs method to translate out of SSA form
    return TranslateOutOfSSAForm(constants_propagated);
  }

  /*
   * Translate into SSA form
   */

  private IRGraph ConvertIntoSSAForm(IRGraph graph) {
    DominatorTree dominatorTree = DominatorTree(graph);
    IRGraph withPhiFunctions = this.InsertPhiFunctions(graph);
    return this.RenameVariables(withPhiFunctions);
  }



  private IRGraph InsertPhiFunctions(IRGraph graph) {
    return graph;
  }

  private IRGraph RenameVariables(IRGraph graph) {
    return graph;
  }

  /*
   * Optimization Methods
   */
  private IRGraph DeadCodeElimination(IRGraph ssa_form) {
    return ssa_form;
  }

  private IRGraph ConstantPropagation(IRGraph ssa_form) {
    return ssa_form;
  }

  /*
   * Translate out of SAA form
   */
  private IRGraph TranslateOutOfSSAForm(IRGraph graph) {
    return graph;
  }

}