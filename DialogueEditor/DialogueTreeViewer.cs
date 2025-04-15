using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MurderMystery.Dialogue;
using MurderMystery.Enums;
using MurderMysteryGame.Dialogue;
using Newtonsoft.Json;

namespace DialogueEditor
{
    public partial class DialogueTreeViewer : Form
    {
        private DialogueRoot _dialogueRoot;
        private TreeView treeView;
        private RichTextBox detailsTextBox;
        private SplitContainer splitContainer;
        private TextBox searchBox;
        private Button saveButton;
        private Button addNodeButton;
        private Button deleteNodeButton;
        private string _dialogueFilePath = "Dialogue.json";

        public DialogueTreeViewer()
        {
            InitializeComponent();

            // Check if we need to look in a different location for the Dialogue.json file
            if (!File.Exists(_dialogueFilePath))
            {
                // Try looking in the parent directory
                string alternativePath = Path.Combine("..", _dialogueFilePath);
                if (File.Exists(alternativePath))
                {
                    _dialogueFilePath = alternativePath;
                }
                else
                {
                    // Allow the user to select the file
                    using (OpenFileDialog dialog = new OpenFileDialog())
                    {
                        dialog.Filter = "JSON files (*.json)|*.json";
                        dialog.Title = "Select Dialogue.json file";

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            _dialogueFilePath = dialog.FileName;
                        }
                    }
                }
            }

            LoadDialogueData();
            PopulateTreeView();
        }

        private void InitializeComponent()
        {
            this.Text = "Dialogue Tree Editor";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create main split container
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 400
            };
            this.Controls.Add(splitContainer);

            // Create toolbar panel
            Panel toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };
            splitContainer.Panel1.Controls.Add(toolbarPanel);

            // Create search box
            searchBox = new TextBox
            {
                PlaceholderText = "Search nodes...",
                Width = 200,
                Location = new Point(10, 10)
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            toolbarPanel.Controls.Add(searchBox);

            // Create buttons
            saveButton = new Button
            {
                Text = "Save",
                Width = 80,
                Location = new Point(220, 10)
            };
            saveButton.Click += SaveButton_Click;
            toolbarPanel.Controls.Add(saveButton);

            addNodeButton = new Button
            {
                Text = "Add Node",
                Width = 80,
                Location = new Point(310, 10)
            };
            addNodeButton.Click += AddNodeButton_Click;
            toolbarPanel.Controls.Add(addNodeButton);

            deleteNodeButton = new Button
            {
                Text = "Delete",
                Width = 80,
                Location = new Point(400, 10)
            };
            deleteNodeButton.Click += DeleteNodeButton_Click;
            toolbarPanel.Controls.Add(deleteNodeButton);

            // Create tree view
            treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                HideSelection = false,
                ShowLines = true,
                ShowNodeToolTips = true,
                Font = new Font("Segoe UI", 9F)
            };
            treeView.AfterSelect += TreeView_AfterSelect;
            splitContainer.Panel1.Controls.Add(treeView);
            treeView.Top = toolbarPanel.Height;
            treeView.Height = splitContainer.Panel1.Height - toolbarPanel.Height;

            // Create details panel
            detailsTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 10F),
                WordWrap = true
            };
            splitContainer.Panel2.Controls.Add(detailsTextBox);
        }

        private void LoadDialogueData()
        {
            try
            {
                string json = File.ReadAllText(_dialogueFilePath);
                _dialogueRoot = JsonConvert.DeserializeObject<DialogueRoot>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dialogue data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _dialogueRoot = new DialogueRoot { DialogueTemplates = new Dictionary<string, DialogueNode>() };
            }
        }

        private void PopulateTreeView()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            // Add root node
            TreeNode rootNode = new TreeNode("Dialogue Templates");
            treeView.Nodes.Add(rootNode);

            // Add node for each dialogue template
            foreach (var template in _dialogueRoot.DialogueTemplates)
            {
                TreeNode nodeItem = new TreeNode(template.Key);
                nodeItem.Tag = template.Value;

                // Find where this node is referenced (incoming links)
                var referencingNodes = FindReferencingNodes(template.Key);
                if (referencingNodes.Count > 0)
                {
                    TreeNode incomingLinksNode = new TreeNode("Referenced By");
                    foreach (var refNode in referencingNodes)
                    {
                        incomingLinksNode.Nodes.Add(new TreeNode(refNode));
                    }
                    nodeItem.Nodes.Add(incomingLinksNode);
                }

                // Add NPC options
                if (template.Value.NpcOptions != null && template.Value.NpcOptions.Count > 0)
                {
                    TreeNode npcOptionsNode = new TreeNode("NPC Options");
                    foreach (var npcOption in template.Value.NpcOptions)
                    {
                        TreeNode personalityNode = new TreeNode($"Personality: {npcOption.Personality}");

                        if (!string.IsNullOrEmpty(npcOption.Text))
                        {
                            personalityNode.Nodes.Add(new TreeNode($"Text: {TruncateText(npcOption.Text, 50)}"));
                        }

                        if (npcOption.Options != null && npcOption.Options.Count > 0)
                        {
                            TreeNode optionsNode = new TreeNode("Response Options");
                            foreach (var option in npcOption.Options)
                            {
                                TreeNode optionNode = new TreeNode($"Fondness: {option.Fondness}");
                                optionNode.Nodes.Add(new TreeNode($"Text: {TruncateText(option.Text, 50)}"));

                                if (!string.IsNullOrEmpty(option.NextNodeID))
                                {
                                    optionNode.Nodes.Add(new TreeNode($"Next Node: {option.NextNodeID}") { ForeColor = Color.Blue });
                                }

                                optionsNode.Nodes.Add(optionNode);
                            }
                            personalityNode.Nodes.Add(optionsNode);
                        }

                        npcOptionsNode.Nodes.Add(personalityNode);
                    }
                    nodeItem.Nodes.Add(npcOptionsNode);
                }

                // Add player options
                if (template.Value.PlayerOptions != null && template.Value.PlayerOptions.Count > 0)
                {
                    TreeNode playerOptionsNode = new TreeNode("Player Options");
                    foreach (var playerOption in template.Value.PlayerOptions)
                    {
                        TreeNode toneNode = new TreeNode($"Tone: {playerOption.Tone ?? "None"}");

                        if (!string.IsNullOrEmpty(playerOption.Text))
                        {
                            toneNode.Nodes.Add(new TreeNode($"Text: {TruncateText(playerOption.Text, 50)}"));
                        }

                        if (!string.IsNullOrEmpty(playerOption.NextNodeID))
                        {
                            toneNode.Nodes.Add(new TreeNode($"Next Node: {playerOption.NextNodeID}") { ForeColor = Color.Blue });
                        }

                        if (playerOption.Variations != null && playerOption.Variations.Count > 0)
                        {
                            TreeNode variationsNode = new TreeNode("Variations");
                            foreach (var variation in playerOption.Variations)
                            {
                                TreeNode variationNode = new TreeNode("Variation");

                                if (variation.TextOptions != null && variation.TextOptions.Count > 0)
                                {
                                    TreeNode textOptionsNode = new TreeNode("Text Options");
                                    foreach (var text in variation.TextOptions)
                                    {
                                        textOptionsNode.Nodes.Add(new TreeNode(TruncateText(text, 50)));
                                    }
                                    variationNode.Nodes.Add(textOptionsNode);
                                }

                                if (!string.IsNullOrEmpty(variation.NextNodeID))
                                {
                                    variationNode.Nodes.Add(new TreeNode($"Next Node: {variation.NextNodeID}") { ForeColor = Color.Blue });
                                }

                                variationsNode.Nodes.Add(variationNode);
                            }
                            toneNode.Nodes.Add(variationsNode);
                        }

                        playerOptionsNode.Nodes.Add(toneNode);
                    }
                    nodeItem.Nodes.Add(playerOptionsNode);
                }

                // Add outgoing links
                var outgoingLinks = FindOutgoingLinks(template.Value);
                if (outgoingLinks.Count > 0)
                {
                    TreeNode outgoingLinksNode = new TreeNode("Outgoing Links");
                    foreach (var link in outgoingLinks)
                    {
                        outgoingLinksNode.Nodes.Add(new TreeNode(link) { ForeColor = Color.Green });
                    }
                    nodeItem.Nodes.Add(outgoingLinksNode);
                }

                rootNode.Nodes.Add(nodeItem);
            }

            rootNode.Expand();
            treeView.EndUpdate();
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + "...";
        }

        private List<string> FindReferencingNodes(string nodeId)
        {
            List<string> referencingNodes = new List<string>();

            foreach (var template in _dialogueRoot.DialogueTemplates)
            {
                bool isReferenced = false;

                // Check player options
                if (template.Value.PlayerOptions != null)
                {
                    foreach (var option in template.Value.PlayerOptions)
                    {
                        if (option.NextNodeID == nodeId)
                        {
                            isReferenced = true;
                            break;
                        }

                        if (option.Variations != null)
                        {
                            foreach (var variation in option.Variations)
                            {
                                if (variation.NextNodeID == nodeId)
                                {
                                    isReferenced = true;
                                    break;
                                }
                            }

                            if (isReferenced) break;
                        }
                    }
                }

                // Check NPC options
                if (!isReferenced && template.Value.NpcOptions != null)
                {
                    foreach (var npcOption in template.Value.NpcOptions)
                    {
                        if (npcOption.Options != null)
                        {
                            foreach (var option in npcOption.Options)
                            {
                                if (option.NextNodeID == nodeId)
                                {
                                    isReferenced = true;
                                    break;
                                }
                            }

                            if (isReferenced) break;
                        }
                    }
                }

                if (isReferenced)
                {
                    referencingNodes.Add(template.Key);
                }
            }

            return referencingNodes;
        }

        private List<string> FindOutgoingLinks(DialogueNode node)
        {
            HashSet<string> links = new HashSet<string>();

            // Check player options
            if (node.PlayerOptions != null)
            {
                foreach (var option in node.PlayerOptions)
                {
                    if (!string.IsNullOrEmpty(option.NextNodeID))
                    {
                        links.Add(option.NextNodeID);
                    }

                    if (option.Variations != null)
                    {
                        foreach (var variation in option.Variations)
                        {
                            if (!string.IsNullOrEmpty(variation.NextNodeID))
                            {
                                links.Add(variation.NextNodeID);
                            }
                        }
                    }
                }
            }

            // Check NPC options
            if (node.NpcOptions != null)
            {
                foreach (var npcOption in node.NpcOptions)
                {
                    if (npcOption.Options != null)
                    {
                        foreach (var option in npcOption.Options)
                        {
                            if (!string.IsNullOrEmpty(option.NextNodeID))
                            {
                                links.Add(option.NextNodeID);
                            }
                        }
                    }
                }
            }

            return links.ToList();
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            detailsTextBox.Clear();

            // Get the node data
            if (e.Node.Tag is DialogueNode dialogueNode)
            {
                string json = JsonConvert.SerializeObject(dialogueNode, Formatting.Indented);
                detailsTextBox.Text = json;
                HighlightJson(detailsTextBox);
            }
            else
            {
                // Try to get parent node tag
                TreeNode parent = e.Node.Parent;
                while (parent != null)
                {
                    if (parent.Tag is DialogueNode parentNode)
                    {
                        string json = JsonConvert.SerializeObject(parentNode, Formatting.Indented);
                        detailsTextBox.Text = json;
                        HighlightJson(detailsTextBox);

                        // Highlight the selected part
                        HighlightRelevantSection(e.Node, detailsTextBox);
                        break;
                    }
                    parent = parent.Parent;
                }
            }
        }

        private void HighlightJson(RichTextBox rtb)
        {
            // Simple JSON syntax highlighting
            string text = rtb.Text;
            rtb.SelectAll();
            rtb.SelectionColor = Color.Black;

            // Highlight property names
            int searchIndex = 0;
            int propertyStart;
            while ((propertyStart = text.IndexOf("\"", searchIndex)) != -1)
            {
                int propertyEnd = text.IndexOf("\":", propertyStart);
                if (propertyEnd != -1)
                {
                    rtb.Select(propertyStart, propertyEnd - propertyStart + 2);
                    rtb.SelectionColor = Color.DarkBlue;
                    searchIndex = propertyEnd + 2;
                }
                else
                {
                    searchIndex = propertyStart + 1;
                }
            }

            // Highlight brackets and braces
            foreach (char c in new[] { '{', '}', '[', ']' })
            {
                searchIndex = 0;
                while ((searchIndex = text.IndexOf(c, searchIndex)) != -1)
                {
                    rtb.Select(searchIndex, 1);
                    rtb.SelectionColor = Color.DarkGray;
                    searchIndex++;
                }
            }

            // Highlight values
            string[] keywords = new[] { "true", "false", "null" };
            foreach (string keyword in keywords)
            {
                searchIndex = 0;
                while ((searchIndex = text.IndexOf(keyword, searchIndex)) != -1)
                {
                    // Check if it's a standalone word
                    bool isWord = (searchIndex == 0 || !char.IsLetterOrDigit(text[searchIndex - 1])) &&
                                 (searchIndex + keyword.Length == text.Length || !char.IsLetterOrDigit(text[searchIndex + keyword.Length]));

                    if (isWord)
                    {
                        rtb.Select(searchIndex, keyword.Length);
                        rtb.SelectionColor = Color.Purple;
                    }
                    searchIndex += keyword.Length;
                }
            }

            rtb.SelectionStart = 0;
            rtb.SelectionLength = 0;
        }

        private void HighlightRelevantSection(TreeNode node, RichTextBox rtb)
        {
            // This is a simplified approach - for a full solution, you'd need more robust JSON parsing
            string searchText = node.Text;

            if (searchText.StartsWith("Tone:"))
                searchText = "\"tone\":";
            else if (searchText.StartsWith("Personality:"))
                searchText = "\"personality\":";
            else if (searchText.StartsWith("Text:"))
                searchText = "\"text\":";
            else if (searchText.StartsWith("Next Node:"))
                searchText = "\"nextNodeID\":";
            else if (searchText == "NPC Options")
                searchText = "\"npcOptions\":";
            else if (searchText == "Player Options")
                searchText = "\"playerOptions\":";
            else if (searchText == "Response Options")
                searchText = "\"options\":";
            else if (searchText == "Variations")
                searchText = "\"variations\":";
            else
                return;

            int index = rtb.Text.IndexOf(searchText);
            if (index != -1)
            {
                // Find the start of the relevant JSON object or array
                int bracketLevel = 0;
                int startPos = index;
                bool foundStart = false;

                while (startPos >= 0 && !foundStart)
                {
                    char c = rtb.Text[startPos];
                    if (c == '}' || c == ']')
                        bracketLevel++;
                    else if (c == '{' || c == '[')
                    {
                        bracketLevel--;
                        if (bracketLevel < 0)
                        {
                            foundStart = true;
                            break;
                        }
                    }
                    startPos--;
                }

                // Find the end of the relevant JSON object or array
                bracketLevel = 0;
                int endPos = index + searchText.Length;
                bool foundEnd = false;

                while (endPos < rtb.Text.Length && !foundEnd)
                {
                    char c = rtb.Text[endPos];
                    if (c == '{' || c == '[')
                        bracketLevel++;
                    else if (c == '}' || c == ']')
                    {
                        bracketLevel--;
                        if (bracketLevel < 0)
                        {
                            foundEnd = true;
                            endPos++;
                            break;
                        }
                    }
                    endPos++;
                }

                if (foundStart && foundEnd)
                {
                    rtb.SelectionStart = startPos + 1;
                    rtb.SelectionLength = endPos - startPos - 1;
                    rtb.SelectionBackColor = Color.LightYellow;
                    rtb.ScrollToCaret();
                }
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                PopulateTreeView();
                return;
            }

            string searchTerm = searchBox.Text.ToLower();
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            // Add root node
            TreeNode rootNode = new TreeNode("Search Results");
            treeView.Nodes.Add(rootNode);

            foreach (var template in _dialogueRoot.DialogueTemplates)
            {
                bool showNode = template.Key.ToLower().Contains(searchTerm);

                // Search in NPC options
                if (!showNode && template.Value.NpcOptions != null)
                {
                    foreach (var npcOption in template.Value.NpcOptions)
                    {
                        if ((npcOption.Personality != null && npcOption.Personality.ToLower().Contains(searchTerm)) ||
                            (npcOption.Text != null && npcOption.Text.ToLower().Contains(searchTerm)))
                        {
                            showNode = true;
                            break;
                        }

                        if (npcOption.Options != null)
                        {
                            foreach (var option in npcOption.Options)
                            {
                                if (option.Text != null && option.Text.ToLower().Contains(searchTerm))
                                {
                                    showNode = true;
                                    break;
                                }
                            }

                            if (showNode) break;
                        }
                    }
                }

                // Search in player options
                if (!showNode && template.Value.PlayerOptions != null)
                {
                    foreach (var playerOption in template.Value.PlayerOptions)
                    {
                        if ((playerOption.Tone != null && playerOption.Tone.ToLower().Contains(searchTerm)) ||
                            (playerOption.Text != null && playerOption.Text.ToLower().Contains(searchTerm)))
                        {
                            showNode = true;
                            break;
                        }

                        if (playerOption.Variations != null)
                        {
                            foreach (var variation in playerOption.Variations)
                            {
                                if (variation.TextOptions != null)
                                {
                                    foreach (var text in variation.TextOptions)
                                    {
                                        if (text.ToLower().Contains(searchTerm))
                                        {
                                            showNode = true;
                                            break;
                                        }
                                    }

                                    if (showNode) break;
                                }
                            }

                            if (showNode) break;
                        }
                    }
                }

                if (showNode)
                {
                    TreeNode nodeItem = new TreeNode(template.Key);
                    nodeItem.Tag = template.Value;
                    rootNode.Nodes.Add(nodeItem);
                }
            }

            rootNode.Expand();
            treeView.EndUpdate();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                string json = JsonConvert.SerializeObject(_dialogueRoot, Formatting.Indented);
                File.WriteAllText(_dialogueFilePath, json);
                MessageBox.Show("Dialogue data saved successfully!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving dialogue data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddNodeButton_Click(object sender, EventArgs e)
        {
            string nodeName = Microsoft.VisualBasic.Interaction.InputBox("Enter new node ID:", "Add Node", "new_node_id");
            if (string.IsNullOrWhiteSpace(nodeName))
                return;

            if (_dialogueRoot.DialogueTemplates.ContainsKey(nodeName))
            {
                MessageBox.Show($"A node with ID '{nodeName}' already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create new empty node
            var newNode = new DialogueNode
            {
                NodeID = nodeName,
                NpcOptions = new List<DialogueVariation>
                {
                    new DialogueVariation
                    {
                        Personality = "Default",
                        Text = "Default response"
                    }
                },
                PlayerOptions = new List<DialogueOption>()
            };

            _dialogueRoot.DialogueTemplates.Add(nodeName, newNode);
            PopulateTreeView();

            // Find and select the new node
            foreach (TreeNode rootNode in treeView.Nodes)
            {
                foreach (TreeNode node in rootNode.Nodes)
                {
                    if (node.Text == nodeName)
                    {
                        treeView.SelectedNode = node;
                        break;
                    }
                }
            }
        }

        private void DeleteNodeButton_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null || treeView.SelectedNode.Tag == null)
                return;

            if (treeView.SelectedNode.Tag is DialogueNode dialogueNode)
            {
                string nodeId = dialogueNode.NodeID;

                if (MessageBox.Show($"Are you sure you want to delete the node '{nodeId}'?",
                                   "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // Check if this node is referenced by other nodes
                    var referencingNodes = FindReferencingNodes(nodeId);
                    if (referencingNodes.Count > 0)
                    {
                        string message = $"This node is referenced by the following nodes:\n{string.Join("\n", referencingNodes)}\n\nDo you still want to delete it?";
                        if (MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            return;
                    }

                    _dialogueRoot.DialogueTemplates.Remove(nodeId);
                    PopulateTreeView();
                }
            }
        }
    }
}